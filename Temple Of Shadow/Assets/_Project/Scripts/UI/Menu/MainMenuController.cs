using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu1Panel;
    [SerializeField] private GameObject menu2Panel;

    private void Start()
    {
        ResolvePanels();
        BindSaveSlots();

        if (menu1Panel != null)
        {
            menu1Panel.SetActive(true);
        }

        if (menu2Panel != null)
        {
            menu2Panel.SetActive(false);
        }
    }

    public void OpenMenu2()
    {
        ResolvePanels();
        RefreshSlotLabels();

        if (menu1Panel != null)
        {
            menu1Panel.SetActive(false);
        }

        if (menu2Panel != null)
        {
            menu2Panel.SetActive(true);
        }
    }

    public void BackToMenu1()
    {
        if (menu2Panel != null)
        {
            menu2Panel.SetActive(false);
        }

        if (menu1Panel != null)
        {
            menu1Panel.SetActive(true);
        }
    }

    public void SelectSaveSlot(int slotIndex)
    {
        SoundManager.Instance?.PlaySFX("click_button");
        GameSession.SelectSlot(slotIndex);
        SceneManager.LoadScene("CharacterSelect");
    }

    public void RefreshSlotLabels()
    {
        for (int slotIndex = 1; slotIndex <= 3; slotIndex++)
        {
            Button button = FindButton($"Slot{slotIndex}Button");
            if (button == null)
            {
                continue;
            }

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label == null)
            {
                continue;
            }

            bool hasProfile = GameProfileStore.TryLoad(slotIndex) != null;
            StyleSaveSlotButton(button, label, hasProfile);

            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 12f;
            label.fontSizeMax = 22f;
            label.enableWordWrapping = true;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.richText = true;
            label.color = Color.white;
            label.outlineWidth = 0.12f;
            label.outlineColor = new Color(0f, 0f, 0f, 0.72f);
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(10f, 6f);
            labelRect.offsetMax = new Vector2(-10f, -6f);

            GameProfileData profile = GameProfileStore.TryLoad(slotIndex);
            label.text = profile == null
                ? $"<color=#F7F1E8>SLOT {slotIndex}</color>\n<color=#E9B14A>NEW JOURNEY</color>"
                : BuildSlotLabel(slotIndex, profile);
        }
    }

    private void ResolvePanels()
    {
        if (menu1Panel == null)
        {
            Transform panel = transform.root.Find("Canvas/Menu1Panel");
            menu1Panel = panel != null ? panel.gameObject : GameObject.Find("Menu1Panel");
        }

        if (menu2Panel == null)
        {
            Transform panel = transform.root.Find("Canvas/Menu2Panel");
            menu2Panel = panel != null ? panel.gameObject : GameObject.Find("Menu2Panel");
        }
    }

    private void BindSaveSlots()
    {
        for (int slotIndex = 1; slotIndex <= 3; slotIndex++)
        {
            int capturedSlot = slotIndex;
            Button button = FindButton($"Slot{slotIndex}Button");
            if (button == null)
            {
                continue;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectSaveSlot(capturedSlot));
        }

        RefreshSlotLabels();
    }

    private Button FindButton(string objectName)
    {
        if (menu2Panel != null)
        {
            foreach (Button button in menu2Panel.GetComponentsInChildren<Button>(true))
            {
                if (button.name == objectName)
                {
                    return button;
                }
            }
        }

        GameObject target = GameObject.Find(objectName);
        return target != null ? target.GetComponent<Button>() : null;
    }

    private void StyleSaveSlotButton(Button button, TMP_Text label, bool hasProfile)
    {
        if (button == null || label == null)
        {
            return;
        }

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.white;
            button.targetGraphic = buttonImage;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.07f, 1.07f, 1.07f, 1f);
        colors.pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f);
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        RectTransform plate = EnsureSlotLabelPlate(button.transform, hasProfile);
        if (label.transform.parent != plate)
        {
            label.transform.SetParent(plate, false);
        }

        label.transform.SetAsLastSibling();
    }

    private RectTransform EnsureSlotLabelPlate(Transform buttonTransform, bool hasProfile)
    {
        Transform existing = buttonTransform.Find("SlotLabelPlate");
        GameObject plateObject;
        if (existing != null)
        {
            plateObject = existing.gameObject;
        }
        else
        {
            plateObject = new GameObject("SlotLabelPlate", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            plateObject.transform.SetParent(buttonTransform, false);
        }

        RectTransform rect = plateObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0.43f);
        rect.offsetMin = new Vector2(14f, 12f);
        rect.offsetMax = new Vector2(-14f, -10f);

        Image image = plateObject.GetComponent<Image>();
        image.color = hasProfile
            ? new Color(0.025f, 0.029f, 0.036f, 0.88f)
            : new Color(0.025f, 0.029f, 0.036f, 0.76f);
        image.raycastTarget = false;

        Outline outline = plateObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = plateObject.AddComponent<Outline>();
        }

        outline.effectColor = new Color(0.72f, 0.62f, 0.43f, hasProfile ? 0.62f : 0.38f);
        outline.effectDistance = new Vector2(1f, -1f);
        outline.useGraphicAlpha = false;

        Shadow shadow = plateObject.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = plateObject.AddComponent<Shadow>();
        }

        shadow.effectColor = new Color(0f, 0f, 0f, 0.48f);
        shadow.effectDistance = new Vector2(4f, -4f);
        plateObject.transform.SetAsLastSibling();
        return rect;
    }

    private string BuildSlotLabel(int slotIndex, GameProfileData profile)
    {
        GameContentCatalog catalog = GameContentCatalog.Load();
        CharacterMenuEntry character = catalog != null ? catalog.GetCharacter(profile.selectedCharacterId) : null;
        LevelMenuEntry level = catalog != null ? catalog.GetLevel(profile.selectedLevelId) : null;
        string characterName = character != null && character.data != null
            ? character.data.characterName
            : "ADVENTURER";
        string levelName = level != null ? level.displayName : "NO STAGE";
        return $"<color=#F7F1E8>SLOT {slotIndex}</color>\n<color=#E9B14A>{characterName.ToUpperInvariant()}</color>\n<color=#CDD5E0>{levelName.ToUpperInvariant()}</color>";
    }
}
