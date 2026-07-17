using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreparationMenuController : MonoBehaviour
{
    private enum MenuTab
    {
        Character,
        Equipment,
        Stage
    }

    private static readonly Color Overlay = new(0.006f, 0.008f, 0.013f, 0.90f);
    private static readonly Color Header = new(0.009f, 0.012f, 0.018f, 0.99f);
    private static readonly Color Panel = new(0.016f, 0.020f, 0.028f, 0.992f);
    private static readonly Color PanelRaised = new(0.034f, 0.040f, 0.052f, 1f);
    private static readonly Color PanelDisabled = new(0.020f, 0.023f, 0.031f, 0.98f);
    private static readonly Color Border = new(0.54f, 0.58f, 0.65f, 0.72f);
    private static readonly Color BorderSoft = new(0.27f, 0.30f, 0.36f, 0.82f);
    private static readonly Color Gold = new(0.72f, 0.42f, 0.10f, 1f);
    private static readonly Color GoldDark = new(0.005f, 0.003f, 0.001f, 1f);
    private static readonly Color TextPrimary = new(0.97f, 0.95f, 0.88f, 1f);
    private static readonly Color TextMuted = new(0.76f, 0.80f, 0.87f, 1f);
    private static readonly Color TextDim = new(0.50f, 0.54f, 0.61f, 1f);
    private static readonly Color Positive = new(0.42f, 0.88f, 0.60f, 1f);
    private static readonly Color Negative = new(0.95f, 0.42f, 0.38f, 1f);

    private GameContentCatalog catalog;
    private GameProfileData profile;
    private MenuTab currentTab;
    private EquipmentSlotType selectedEquipmentSlot = EquipmentSlotType.Sword;

    private RectTransform root;
    private RectTransform tabBar;
    private RectTransform body;
    private TMP_Text footerText;
    private Button beginButton;
    private RawImage previewImage;
    private TMP_Text equipmentDetailText;

    private GameObject previewCameraObject;
    private GameObject previewCharacter;
    private RenderTexture previewTexture;

    private void Awake()
    {
        if (!gameObject.scene.IsValid())
        {
            return;
        }

        catalog = GameContentCatalog.Load();
        profile = GameSession.EnsureProfile();
        if (catalog == null || profile == null)
        {
            Debug.LogError("[PreparationMenu] GameContentCatalog or active profile is missing.");
            return;
        }

        EnsureEventSystem();
        ConfigureCanvas();
        BuildShell();
        ShowTab(MenuTab.Character);
    }

    private void OnDestroy()
    {
        DestroyCharacterPreview();
    }

    private void ConfigureCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private void BuildShell()
    {
        Transform existing = transform.Find("PreparationMenuRoot");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        root = CreatePanel("PreparationMenuRoot", transform, Overlay, false);
        Stretch(root);

        RectTransform header = CreatePanel("Header", root, Header, true);
        SetRect(header, new Vector2(0f, 1f), Vector2.one, new Vector2(32f, -98f), new Vector2(-32f, -24f));

        Button backButton = CreateButton(header, "BACK", BackToMainMenu);
        SetRect(backButton.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(18f, 14f), new Vector2(158f, -14f));

        TMP_Text title = CreateText(header, "PREPARE EXPEDITION", 30f, FontStyles.Bold, TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0.25f, 0f), new Vector2(0.75f, 1f), Vector2.zero, Vector2.zero);

        TMP_Text slotLabel = CreateText(header, $"SAVE SLOT {profile.slotIndex}", 18f, FontStyles.Bold, TextAlignmentOptions.MidlineRight);
        slotLabel.color = Gold;
        SetRect(slotLabel.rectTransform, new Vector2(0.75f, 0f), new Vector2(1f, 1f),
            new Vector2(0f, 0f), new Vector2(-22f, 0f));

        tabBar = CreatePanel("Tabs", root, Header, true);
        SetRect(tabBar, new Vector2(0f, 1f), Vector2.one, new Vector2(32f, -168f), new Vector2(-32f, -108f));

        body = CreatePanel("Body", root, Color.clear, false);
        SetRect(body, Vector2.zero, Vector2.one, new Vector2(32f, 126f), new Vector2(-32f, -180f));

        RectTransform footer = CreatePanel("Footer", root, Header, true);
        SetRect(footer, Vector2.zero, new Vector2(1f, 0f), new Vector2(32f, 24f), new Vector2(-32f, 108f));

        footerText = CreateText(footer, string.Empty, 20f, FontStyles.Normal, TextAlignmentOptions.MidlineLeft);
        SetRect(footerText.rectTransform, Vector2.zero, new Vector2(0.76f, 1f),
            new Vector2(24f, 0f), new Vector2(-12f, 0f));

        beginButton = CreateButton(footer, "BEGIN EXPEDITION", BeginExpedition, true);
        SetRect(beginButton.GetComponent<RectTransform>(), new Vector2(0.76f, 0f), Vector2.one,
            new Vector2(12f, 14f), new Vector2(-18f, -14f));
    }

    private void ShowTab(MenuTab tab)
    {
        currentTab = tab;
        DestroyCharacterPreview();
        ClearChildren(tabBar);
        ClearChildren(body);
        BuildTabs();

        switch (tab)
        {
            case MenuTab.Character:
                BuildCharacterTab();
                break;
            case MenuTab.Equipment:
                BuildEquipmentTab();
                break;
            case MenuTab.Stage:
                BuildStageTab();
                break;
        }

        UpdateFooter();
    }

    private void BuildTabs()
    {
        HorizontalLayoutGroup layout = tabBar.GetComponent<HorizontalLayoutGroup>();
        if (layout == null)
        {
            layout = tabBar.gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        layout.padding = new RectOffset(8, 8, 7, 7);
        layout.spacing = 8f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        CreateButton(tabBar, "CHARACTER", () => ShowTab(MenuTab.Character), currentTab == MenuTab.Character);
        CreateButton(tabBar, "EQUIPMENT", () => ShowTab(MenuTab.Equipment), currentTab == MenuTab.Equipment);
        CreateButton(tabBar, "STAGE", () => ShowTab(MenuTab.Stage), currentTab == MenuTab.Stage);
    }

    private void BuildCharacterTab()
    {
        RectTransform listPanel = CreateColumn(body, "CharacterList", 0f, 0.235f);
        RectTransform previewPanel = CreateColumn(body, "CharacterPreview", 0.245f, 0.665f);
        RectTransform detailPanel = CreateColumn(body, "CharacterDetails", 0.675f, 1f);

        AddSectionTitle(listPanel, "CHOOSE CHARACTER", "Select a playable class");
        RectTransform list = CreateVerticalList(listPanel, 104f, 22f);

        foreach (CharacterMenuEntry entry in catalog.characters)
        {
            if (entry == null || entry.data == null)
            {
                continue;
            }

            bool selected = string.Equals(entry.Id, profile.selectedCharacterId, StringComparison.OrdinalIgnoreCase);
            bool interactable = entry.available && entry.playerPrefab != null;
            string role = string.IsNullOrWhiteSpace(entry.role) ? "ADVENTURER" : entry.role.ToUpperInvariant();
            string label = BuildMenuButtonLabel(
                entry.data.characterName.ToUpperInvariant(),
                role,
                selected,
                interactable,
                interactable ? null : "LOCKED",
                ColorHex(Negative));

            CharacterMenuEntry captured = entry;
            CreateButton(list, label, () => SelectCharacter(captured), selected, interactable, 94f);
        }

        CharacterMenuEntry character = GetSelectedCharacter();
        BuildCharacterPreview(previewPanel, character);
        BuildCharacterDetails(detailPanel, character);
    }

    private void BuildCharacterPreview(RectTransform panel, CharacterMenuEntry character)
    {
        string characterName = character != null && character.data != null
            ? character.data.characterName.ToUpperInvariant()
            : "NO CHARACTER";
        TMP_Text name = CreateText(panel, characterName, 32f, FontStyles.Bold, TextAlignmentOptions.Center);
        name.color = Gold;
        SetRect(name.rectTransform, new Vector2(0f, 1f), Vector2.one,
            new Vector2(20f, -76f), new Vector2(-20f, -22f));

        GameObject previewObject = new("LiveCharacterPreview", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        previewObject.layer = gameObject.layer;
        previewObject.transform.SetParent(panel, false);
        previewImage = previewObject.GetComponent<RawImage>();
        previewImage.color = Color.white;
        SetRect(previewObject.GetComponent<RectTransform>(), new Vector2(0f, 0.18f), new Vector2(1f, 0.88f),
            new Vector2(28f, 0f), new Vector2(-28f, 0f));

        TMP_Text hint = CreateText(panel, "CURRENT LOADOUT PREVIEW", 16f, FontStyles.Bold, TextAlignmentOptions.Center);
        hint.color = TextMuted;
        SetRect(hint.rectTransform, Vector2.zero, new Vector2(1f, 0.18f),
            new Vector2(16f, 12f), new Vector2(-16f, -8f));

        ShowCharacterPreview(character);
    }

    private void BuildCharacterDetails(RectTransform panel, CharacterMenuEntry character)
    {
        AddSectionTitle(panel, "COMBAT PROFILE", character != null ? character.role : string.Empty);

        TMP_Text description = CreateText(panel,
            character != null && !string.IsNullOrWhiteSpace(character.description)
                ? character.description
                : "Choose a character to inspect their combat profile.",
            18f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        description.color = TextMuted;
        SetRect(description.rectTransform, new Vector2(0f, 0.70f), new Vector2(1f, 0.86f),
            new Vector2(24f, 0f), new Vector2(-24f, 0f));

        StatSnapshot stats = CalculateStats(character);
        TMP_Text statText = CreateText(panel, BuildStatsText(stats), 22f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        statText.richText = true;
        SetRect(statText.rectTransform, new Vector2(0f, 0.12f), new Vector2(1f, 0.68f),
            new Vector2(24f, 0f), new Vector2(-24f, 0f));
    }

    private void BuildEquipmentTab()
    {
        RectTransform slotsPanel = CreateColumn(body, "EquipmentSlots", 0f, 0.25f);
        RectTransform previewPanel = CreateColumn(body, "EquipmentPreview", 0.26f, 0.63f);
        RectTransform inventoryPanel = CreateColumn(body, "EquipmentInventory", 0.64f, 1f);

        AddSectionTitle(slotsPanel, "LOADOUT", "Select an equipment slot");
        RectTransform slotList = CreateVerticalList(slotsPanel, 104f, 22f);
        EquipmentSlotType[] slots =
        {
            EquipmentSlotType.Sword,
            EquipmentSlotType.Armor,
            EquipmentSlotType.Accessory,
            EquipmentSlotType.Projectile
        };

        foreach (EquipmentSlotType slot in slots)
        {
            EquipmentData equipped = GetEquipped(slot);
            string itemName = equipped != null ? equipped.itemName : "EMPTY";
            string label = BuildMenuButtonLabel(slot.ToString().ToUpperInvariant(), itemName, selectedEquipmentSlot == slot, true);
            EquipmentSlotType captured = slot;
            CreateButton(slotList, label, () => SelectEquipmentSlot(captured), selectedEquipmentSlot == slot, true, 88f);
        }

        CharacterMenuEntry character = GetSelectedCharacter();
        BuildCharacterPreview(previewPanel, character);

        BuildEquipmentInventory(inventoryPanel);
    }

    private void BuildEquipmentInventory(RectTransform panel)
    {
        EquipmentData current = GetEquipped(selectedEquipmentSlot);
        string subtitle = current != null ? $"Equipped: {current.itemName}" : "Nothing equipped";
        AddSectionTitle(panel, $"BACKPACK / {selectedEquipmentSlot.ToString().ToUpperInvariant()}", subtitle);

        RectTransform itemList = CreateVerticalList(panel, 112f, 220f);
        List<EquipmentData> candidates = GetInventoryEquipment(selectedEquipmentSlot);
        if (candidates.Count == 0)
        {
            TMP_Text empty = CreateText(itemList, "No compatible equipment in this inventory.", 18f,
                FontStyles.Normal, TextAlignmentOptions.Center);
            empty.color = TextMuted;
            empty.gameObject.AddComponent<LayoutElement>().preferredHeight = 110f;
        }

        foreach (EquipmentData equipment in candidates)
        {
            EquipmentData captured = equipment;
            Button button = CreateItemButton(itemList, equipment, () => EquipFromInventory(captured));
            AddPointerEnter(button.gameObject, () => ShowEquipmentComparison(captured));
        }

        equipmentDetailText = CreateText(panel, string.Empty, 18f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        equipmentDetailText.richText = true;
        SetRect(equipmentDetailText.rectTransform, Vector2.zero, new Vector2(1f, 0f),
            new Vector2(24f, 24f), new Vector2(-24f, 198f));

        if (candidates.Count > 0)
        {
            ShowEquipmentComparison(candidates[0]);
        }
        else
        {
            equipmentDetailText.text = "Collect compatible equipment during a run to make it available here.";
            equipmentDetailText.color = TextMuted;
        }
    }

    private void BuildStageTab()
    {
        RectTransform listPanel = CreateColumn(body, "StageList", 0f, 0.29f);
        RectTransform stagePanel = CreateColumn(body, "StagePreview", 0.30f, 0.70f);
        RectTransform detailsPanel = CreateColumn(body, "StageDetails", 0.71f, 1f);

        AddSectionTitle(listPanel, "SELECT STAGE", "Choose your destination");
        RectTransform list = CreateVerticalList(listPanel, 104f, 22f);

        foreach (LevelMenuEntry level in catalog.levels)
        {
            if (level == null)
            {
                continue;
            }

            bool unlocked = profile.IsLevelUnlocked(level.Id);
            bool selected = string.Equals(level.Id, profile.selectedLevelId, StringComparison.OrdinalIgnoreCase);
            string state = unlocked ? level.difficulty : "LOCKED";
            string label = BuildMenuButtonLabel(level.displayName.ToUpperInvariant(), state, selected, unlocked);
            LevelMenuEntry captured = level;
            CreateButton(list, label, () => SelectLevel(captured), selected, unlocked, 94f);
        }

        LevelMenuEntry selectedLevel = GetSelectedLevel();
        BuildStagePreview(stagePanel, selectedLevel);
        BuildStageDetails(detailsPanel, selectedLevel);
    }

    private void BuildStagePreview(RectTransform panel, LevelMenuEntry level)
    {
        GameObject imageObject = new("StageImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.layer = gameObject.layer;
        imageObject.transform.SetParent(panel, false);
        Image image = imageObject.GetComponent<Image>();
        image.sprite = level != null && level.thumbnail != null
            ? level.thumbnail
            : Resources.Load<Sprite>("Background_menu2");
        image.preserveAspect = false;
        image.color = new Color(0.84f, 0.88f, 0.94f, 1f);
        SetRect(imageObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.one,
            new Vector2(14f, 14f), new Vector2(-14f, -14f));

        RectTransform shade = CreatePanel("StageShade", imageObject.transform, new Color(0f, 0f, 0f, 0.34f), false);
        Stretch(shade);

        string stageName = level != null ? level.displayName.ToUpperInvariant() : "NO STAGE";
        TMP_Text title = CreateText(shade, stageName, 36f, FontStyles.Bold, TextAlignmentOptions.BottomLeft);
        title.color = TextPrimary;
        SetRect(title.rectTransform, Vector2.zero, new Vector2(1f, 0.32f),
            new Vector2(32f, 24f), new Vector2(-32f, -8f));
    }

    private void BuildStageDetails(RectTransform panel, LevelMenuEntry level)
    {
        AddSectionTitle(panel, "EXPEDITION", level != null ? level.difficulty : string.Empty);

        TMP_Text description = CreateText(panel,
            level != null ? level.description : "Choose a stage to inspect it.",
            19f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        description.color = TextMuted;
        SetRect(description.rectTransform, new Vector2(0f, 0.52f), new Vector2(1f, 0.84f),
            new Vector2(24f, 0f), new Vector2(-24f, 0f));

        CharacterMenuEntry character = GetSelectedCharacter();
        StatSnapshot stats = CalculateStats(character);
        string readiness =
            $"<color=#{ColorHex(Gold)}>SELECTED LOADOUT</color>\n\n" +
            $"Character     {GetCharacterName(character)}\n" +
            $"Equipment     {GetEquipmentCount()}/4\n" +
            $"Total HP      {stats.hp}\n" +
            $"Damage        {stats.damage}\n" +
            $"Armor         {stats.armor}";
        TMP_Text readinessText = CreateText(panel, readiness, 20f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        readinessText.richText = true;
        SetRect(readinessText.rectTransform, new Vector2(0f, 0.10f), new Vector2(1f, 0.50f),
            new Vector2(24f, 0f), new Vector2(-24f, 0f));
    }

    private void SelectCharacter(CharacterMenuEntry character)
    {
        if (character == null || !character.available || character.playerPrefab == null)
        {
            return;
        }

        PlayClick();
        profile.selectedCharacterId = character.Id;
        GameSession.Save();
        ShowTab(MenuTab.Character);
    }

    private void SelectEquipmentSlot(EquipmentSlotType slot)
    {
        PlayClick();
        selectedEquipmentSlot = slot;
        ShowTab(MenuTab.Equipment);
    }

    private void EquipFromInventory(EquipmentData equipment)
    {
        if (equipment == null || equipment.SlotType == EquipmentSlotType.None)
        {
            return;
        }

        string newItemId = GameContentCatalog.GetItemId(equipment);
        if (!profile.RemoveItem(newItemId))
        {
            return;
        }

        string previousItemId = profile.GetEquippedItemId(equipment.SlotType);
        if (!string.IsNullOrWhiteSpace(previousItemId))
        {
            profile.AddItem(previousItemId);
        }

        PlayClick();
        profile.SetEquippedItem(equipment.SlotType, newItemId);
        GameSession.Save();
        ShowTab(MenuTab.Equipment);
    }

    private void SelectLevel(LevelMenuEntry level)
    {
        if (level == null || !profile.IsLevelUnlocked(level.Id))
        {
            return;
        }

        PlayClick();
        profile.selectedLevelId = level.Id;
        GameSession.Save();
        ShowTab(MenuTab.Stage);
    }

    private void BackToMainMenu()
    {
        PlayClick();
        GameSession.Save();
        SceneManager.LoadScene("MainMenu");
    }

    private void BeginExpedition()
    {
        CharacterMenuEntry character = GetSelectedCharacter();
        LevelMenuEntry level = GetSelectedLevel();
        if (character == null || level == null || !profile.IsLevelUnlocked(level.Id))
        {
            return;
        }

        PlayClick();
        GameSession.Save();
        SceneManager.LoadScene(level.sceneName);
    }

    private void UpdateFooter()
    {
        CharacterMenuEntry character = GetSelectedCharacter();
        LevelMenuEntry level = GetSelectedLevel();
        string levelName = level != null ? level.displayName : "NO STAGE";
        footerText.text =
            $"<color=#{ColorHex(Gold)}>{GetCharacterName(character)}</color>  |  " +
            $"{GetEquipmentCount()}/4 EQUIPPED  |  {levelName.ToUpperInvariant()}";
        footerText.richText = true;

        bool ready = character != null && character.available && character.playerPrefab != null
            && level != null && profile.IsLevelUnlocked(level.Id);
        beginButton.interactable = ready;
        StyleBeginButton(ready);
    }

    private void StyleBeginButton(bool ready)
    {
        if (beginButton == null)
        {
            return;
        }

        Image image = beginButton.GetComponent<Image>();
        if (image != null)
        {
            image.color = ready ? Gold : PanelDisabled;
        }

        Outline outline = beginButton.GetComponent<Outline>();
        if (outline != null)
        {
            outline.effectColor = ready ? Gold : BorderSoft;
        }

        TMP_Text text = beginButton.GetComponentInChildren<TMP_Text>(true);
        if (text == null)
        {
            return;
        }

        text.color = ready ? GoldDark : TextDim;
        text.outlineWidth = ready ? 0f : 0.10f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.68f);
    }

    private CharacterMenuEntry GetSelectedCharacter()
    {
        return catalog.GetCharacter(profile.selectedCharacterId) ?? catalog.GetFirstAvailableCharacter();
    }

    private LevelMenuEntry GetSelectedLevel()
    {
        return catalog.GetLevel(profile.selectedLevelId) ?? catalog.GetFirstDefaultLevel();
    }

    private EquipmentData GetEquipped(EquipmentSlotType slot)
    {
        return catalog.GetItem(profile.GetEquippedItemId(slot)) as EquipmentData;
    }

    private List<EquipmentData> GetInventoryEquipment(EquipmentSlotType slot)
    {
        List<EquipmentData> results = new();
        foreach (InventoryEntryData entry in profile.inventory)
        {
            if (entry.amount <= 0 || catalog.GetItem(entry.itemId) is not EquipmentData equipment)
            {
                continue;
            }

            if (equipment.SlotType == slot)
            {
                results.Add(equipment);
            }
        }

        return results;
    }

    private int GetEquipmentCount()
    {
        int count = 0;
        EquipmentSlotType[] slots =
        {
            EquipmentSlotType.Sword,
            EquipmentSlotType.Armor,
            EquipmentSlotType.Accessory,
            EquipmentSlotType.Projectile
        };

        foreach (EquipmentSlotType slot in slots)
        {
            if (GetEquipped(slot) != null)
            {
                count++;
            }
        }

        return count;
    }

    private string GetCharacterName(CharacterMenuEntry character)
    {
        return character != null && character.data != null ? character.data.characterName.ToUpperInvariant() : "NO CHARACTER";
    }

    private StatSnapshot CalculateStats(CharacterMenuEntry character)
    {
        StatSnapshot stats = new();
        if (character != null && character.data != null)
        {
            stats.hp = character.data.maxHp;
            stats.damage = character.data.damage;
            stats.armor = character.data.armor;
            stats.moveSpeed = character.data.moveSpeed;
            stats.attackSpeed = character.data.attackSpeed;
            stats.jumpForce = character.data.jumpForce;
        }

        foreach (EquipmentEntryData equipmentEntry in profile.equipment)
        {
            if (catalog.GetItem(equipmentEntry.itemId) is not EquipmentData equipment)
            {
                continue;
            }

            stats.hp += equipment.maxHP;
            stats.damage += equipment.damage;
            stats.armor += equipment.armor;
            stats.moveSpeed += equipment.moveSpeed;
            stats.attackSpeed += equipment.attackSpeed;
            stats.jumpForce += equipment.jumpForce;
        }

        return stats;
    }

    private string BuildStatsText(StatSnapshot stats)
    {
        string muted = ColorHex(TextMuted);
        return
            $"<color=#{muted}>HEALTH</color>        <b>{stats.hp}</b>\n\n" +
            $"<color=#{muted}>DAMAGE</color>        <b>{stats.damage}</b>\n\n" +
            $"<color=#{muted}>ARMOR</color>         <b>{stats.armor}</b>\n\n" +
            $"<color=#{muted}>MOVE</color>          <b>{stats.moveSpeed:0.#}</b>\n\n" +
            $"<color=#{muted}>ATTACK SPEED</color>  <b>{stats.attackSpeed:0.#}</b>\n\n" +
            $"<color=#{muted}>JUMP</color>          <b>{stats.jumpForce:0.#}</b>";
    }

    private void ShowEquipmentComparison(EquipmentData candidate)
    {
        if (equipmentDetailText == null || candidate == null)
        {
            return;
        }

        EquipmentData current = GetEquipped(candidate.SlotType);
        string currentName = current != null ? current.itemName : "Empty slot";
        equipmentDetailText.color = TextPrimary;
        equipmentDetailText.text =
            $"<color=#{ColorHex(Gold)}>{candidate.itemName.ToUpperInvariant()}</color>\n" +
            $"Replaces {currentName}\n\n" +
            BuildDifference("Health", candidate.maxHP - (current != null ? current.maxHP : 0)) + "\n" +
            BuildDifference("Damage", candidate.damage - (current != null ? current.damage : 0)) + "\n" +
            BuildDifference("Armor", candidate.armor - (current != null ? current.armor : 0)) + "\n" +
            BuildDifference("Move", candidate.moveSpeed - (current != null ? current.moveSpeed : 0f));
    }

    private string BuildDifference(string label, float value)
    {
        string color = value > 0f ? ColorHex(Positive) : value < 0f ? ColorHex(Negative) : ColorHex(TextMuted);
        string sign = value > 0f ? "+" : string.Empty;
        return $"{label,-10} <color=#{color}>{sign}{value:0.#}</color>";
    }

    private string BuildItemStats(EquipmentData equipment)
    {
        List<string> values = new();
        if (equipment.damage != 0) values.Add($"DMG {equipment.damage:+0;-0}");
        if (equipment.armor != 0) values.Add($"ARM {equipment.armor:+0;-0}");
        if (equipment.maxHP != 0) values.Add($"HP {equipment.maxHP:+0;-0}");
        if (!Mathf.Approximately(equipment.moveSpeed, 0f)) values.Add($"MOV {equipment.moveSpeed:+0.#;-0.#}");
        return values.Count > 0 ? string.Join("  ", values) : "NO STAT CHANGE";
    }

    private string BuildMenuButtonLabel(
        string primary,
        string secondary,
        bool selected,
        bool interactable,
        string status = null,
        string statusColor = null)
    {
        string primaryColor = selected
            ? "050301"
            : interactable ? ColorHex(TextPrimary) : ColorHex(TextDim);
        string secondaryColor = selected
            ? "211403"
            : interactable ? ColorHex(TextMuted) : "7B828F";

        string label = $"<color=#{primaryColor}>{primary}</color>";
        if (!string.IsNullOrWhiteSpace(secondary))
        {
            label += $"\n<color=#{secondaryColor}>{secondary}</color>";
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            label += $"\n<color=#{(statusColor ?? ColorHex(Negative))}>{status}</color>";
        }

        return label;
    }

    private static string ColorHex(Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }

    private void ShowCharacterPreview(CharacterMenuEntry character)
    {
        if (previewImage == null || character == null || character.playerPrefab == null)
        {
            return;
        }

        const int previewLayer = 30;
        previewTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32)
        {
            name = "CharacterPreviewTexture",
            antiAliasing = 2
        };
        previewTexture.Create();
        previewImage.texture = previewTexture;

        previewCameraObject = new GameObject("CharacterPreviewCamera");
        Camera camera = previewCameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;
        camera.cullingMask = 1 << previewLayer;
        camera.targetTexture = previewTexture;
        camera.allowHDR = false;
        camera.allowMSAA = true;

        previewCharacter = Instantiate(character.playerPrefab, Vector3.zero, Quaternion.identity);
        previewCharacter.name = "CharacterPreviewModel";
        SetLayerRecursively(previewCharacter.transform, previewLayer);

        HashSet<string> gameplayBehaviours = new()
        {
            nameof(PlayerController),
            nameof(PlayerAnimation),
            nameof(PlayerCombat),
            nameof(PlayerHealth),
            nameof(PlayerWallet),
            nameof(PlayerStats),
            nameof(PlayerBonus),
            nameof(EquipmentVisualManager),
            nameof(DamagePopupSpawner),
            nameof(PlayerAnimationEventRelay)
        };

        foreach (MonoBehaviour behaviour in previewCharacter.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (gameplayBehaviours.Contains(behaviour.GetType().Name))
            {
                behaviour.enabled = false;
            }
        }

        foreach (Rigidbody2D body2D in previewCharacter.GetComponentsInChildren<Rigidbody2D>(true))
        {
            body2D.simulated = false;
        }

        foreach (Collider2D collider in previewCharacter.GetComponentsInChildren<Collider2D>(true))
        {
            collider.enabled = false;
        }

        Renderer[] renderers = previewCharacter.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.orthographicSize = 4f;
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z - 10f);
        camera.orthographicSize = Mathf.Max(2f, Mathf.Max(bounds.extents.y, bounds.extents.x) * 1.35f);
    }

    private void DestroyCharacterPreview()
    {
        if (previewImage != null)
        {
            previewImage.texture = null;
            previewImage = null;
        }

        if (previewCameraObject != null)
        {
            Camera camera = previewCameraObject.GetComponent<Camera>();
            if (camera != null)
            {
                camera.targetTexture = null;
            }

            Destroy(previewCameraObject);
            previewCameraObject = null;
        }

        if (previewCharacter != null)
        {
            Destroy(previewCharacter);
            previewCharacter = null;
        }

        if (previewTexture != null)
        {
            previewTexture.Release();
            Destroy(previewTexture);
            previewTexture = null;
        }
    }

    private RectTransform CreateColumn(RectTransform parent, string name, float minX, float maxX)
    {
        RectTransform panel = CreatePanel(name, parent, Panel, true);
        SetRect(panel, new Vector2(minX, 0f), new Vector2(maxX, 1f), Vector2.zero, Vector2.zero);
        return panel;
    }

    private void AddSectionTitle(RectTransform panel, string title, string subtitle)
    {
        RectTransform headerPlate = CreatePanel("SectionHeader", panel, Header, false);
        SetRect(headerPlate, new Vector2(0f, 1f), Vector2.one,
            new Vector2(14f, -98f), new Vector2(-14f, -14f));

        RectTransform accent = CreatePanel("SectionAccent", headerPlate, Gold, false);
        SetRect(accent, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(5f, 0f));

        TMP_Text titleText = CreateText(headerPlate, title, 23f, FontStyles.Bold, TextAlignmentOptions.TopLeft);
        titleText.color = TextPrimary;
        SetRect(titleText.rectTransform, new Vector2(0f, 1f), Vector2.one,
            new Vector2(20f, -42f), new Vector2(-18f, -10f));

        TMP_Text subtitleText = CreateText(headerPlate, subtitle ?? string.Empty, 15f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        subtitleText.color = TextMuted;
        SetRect(subtitleText.rectTransform, new Vector2(0f, 1f), Vector2.one,
            new Vector2(20f, -68f), new Vector2(-18f, -42f));
    }

    private RectTransform CreateVerticalList(RectTransform panel, float topOffset, float bottomOffset)
    {
        RectTransform list = CreatePanel("List", panel, Color.clear, false);
        SetRect(list, Vector2.zero, Vector2.one,
            new Vector2(18f, bottomOffset), new Vector2(-18f, -topOffset));
        VerticalLayoutGroup layout = list.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 12f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        return list;
    }

    private RectTransform CreatePanel(string name, Transform parent, Color color, bool outline)
    {
        GameObject panelObject = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.layer = gameObject.layer;
        panelObject.transform.SetParent(parent, false);
        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = color.a > 0f;

        if (outline)
        {
            Outline border = panelObject.AddComponent<Outline>();
            border.effectColor = Border;
            border.effectDistance = new Vector2(1f, -1f);
            border.useGraphicAlpha = false;

            Shadow shadow = panelObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.36f);
            shadow.effectDistance = new Vector2(5f, -5f);
            shadow.useGraphicAlpha = true;
        }

        return panelObject.GetComponent<RectTransform>();
    }

    private TMP_Text CreateText(
        Transform parent,
        string value,
        float size,
        FontStyles style,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.layer = gameObject.layer;
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = TextPrimary;
        text.raycastTarget = false;
        text.richText = true;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.characterSpacing = 0f;
        text.outlineWidth = 0.11f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.62f);
        return text;
    }

    private Button CreateButton(
        Transform parent,
        string label,
        Action onClick,
        bool selected = false,
        bool interactable = true,
        float preferredHeight = 52f)
    {
        GameObject buttonObject = new("Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.layer = gameObject.layer;
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.GetComponent<Image>();
        image.color = interactable ? (selected ? Gold : PanelRaised) : PanelDisabled;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = selected ? Gold : interactable ? BorderSoft : new Color(0.16f, 0.17f, 0.19f, 0.85f);
        outline.effectDistance = new Vector2(1f, -1f);
        outline.useGraphicAlpha = false;

        Shadow shadow = buttonObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.28f);
        shadow.effectDistance = new Vector2(4f, -4f);

        Button button = buttonObject.GetComponent<Button>();
        button.interactable = interactable;
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
        colors.disabledColor = new Color(0.72f, 0.72f, 0.72f, 0.95f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.onClick.AddListener(() => onClick?.Invoke());

        LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
        layout.preferredHeight = preferredHeight;
        layout.minHeight = preferredHeight;

        TMP_Text text = CreateText(buttonObject.transform, label, 17f, FontStyles.Bold, TextAlignmentOptions.Center);
        text.color = selected ? GoldDark : interactable ? TextPrimary : TextDim;
        text.enableAutoSizing = true;
        text.fontSizeMin = 12f;
        text.fontSizeMax = preferredHeight >= 80f ? 18f : 17f;
        text.outlineWidth = selected ? 0f : 0.10f;
        text.outlineColor = selected ? new Color(1f, 0.86f, 0.55f, 0.18f) : new Color(0f, 0f, 0f, 0.68f);
        Stretch(text.rectTransform, 12f, 8f);
        return button;
    }

    private Button CreateItemButton(Transform parent, EquipmentData equipment, Action onClick)
    {
        Button button = CreateButton(parent, string.Empty, onClick, false, true, 82f);
        RectTransform buttonRect = button.GetComponent<RectTransform>();

        if (equipment.icon != null)
        {
            GameObject iconObject = new("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObject.layer = gameObject.layer;
            iconObject.transform.SetParent(buttonRect, false);
            Image icon = iconObject.GetComponent<Image>();
            icon.sprite = equipment.icon;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            SetRect(iconObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(0f, 1f),
                new Vector2(12f, 10f), new Vector2(72f, -10f));
        }

        TMP_Text label = CreateText(buttonRect,
            BuildMenuButtonLabel(equipment.itemName.ToUpperInvariant(), BuildItemStats(equipment), false, true),
            16f, FontStyles.Bold, TextAlignmentOptions.MidlineLeft);
        SetRect(label.rectTransform, Vector2.zero, Vector2.one,
            new Vector2(82f, 8f), new Vector2(-12f, -8f));
        return button;
    }

    private void AddPointerEnter(GameObject target, Action callback)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new() { eventID = EventTriggerType.PointerEnter };
        entry.callback.AddListener(_ => callback?.Invoke());
        trigger.triggers.Add(entry);
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        Type inputModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputModuleType != null)
        {
            eventSystemObject.AddComponent(inputModuleType);
        }
        else
        {
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }

    private void PlayClick()
    {
        SoundManager.Instance?.PlaySFX("click_button");
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void SetLayerRecursively(Transform target, int layer)
    {
        target.gameObject.layer = layer;
        foreach (Transform child in target)
        {
            SetLayerRecursively(child, layer);
        }
    }

    private void Stretch(RectTransform rect, float horizontalPadding = 0f, float verticalPadding = 0f)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(horizontalPadding, verticalPadding);
        rect.offsetMax = new Vector2(-horizontalPadding, -verticalPadding);
    }

    private void SetRect(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.localScale = Vector3.one;
    }

    private struct StatSnapshot
    {
        public int hp;
        public int damage;
        public int armor;
        public float moveSpeed;
        public float attackSpeed;
        public float jumpForce;
    }
}
