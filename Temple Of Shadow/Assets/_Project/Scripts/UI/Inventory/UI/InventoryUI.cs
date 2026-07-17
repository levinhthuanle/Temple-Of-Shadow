using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    public InventoryManager inventoryManager;

    public InventorySlotUI[] slots;

    private bool isOpen;
    private InventoryManager subscribedInventoryManager;
    private StatsPanelUI statsPanelUI;

    public EquipmentManager equipmentManager;

    public TooltipUI tooltipUI;

    private void Awake()
    {
        if (!gameObject.scene.IsValid())
        {
            return;
        }

        ResolveReferences();
        ApplyVisualStyle();
        Refresh();
    }

    private void OnEnable()
    {
        if (!gameObject.scene.IsValid())
        {
            return;
        }

        ResolveReferences();
        ApplyVisualStyle();
        SubscribeToInventoryManager();
    }

    private void OnDisable()
    {
        if (subscribedInventoryManager != null)
        {
            subscribedInventoryManager.InventoryChanged -= Refresh;
            subscribedInventoryManager = null;
        }

        if (tooltipUI != null)
        {
            tooltipUI.Hide();
        }
    }

    private void Update()
    {
        if (!gameObject.scene.IsValid())
        {
            return;
        }

        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
        {
            ApplyVisualStyle();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
            Refresh();
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
        }
    }

    public void Refresh()
    {
        if (!gameObject.scene.IsValid())
        {
            return;
        }

        ResolveReferences();
        ApplyVisualStyle();

        if (inventoryManager == null)
        {
            Debug.LogWarning("[InventoryUI] Missing InventoryManager. Add InventoryManager to the scene or assign it in the Inspector.");
            return;
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("[InventoryUI] No inventory slots found. Put Slot1, Slot2... under InventoryPanel/InventoryGrid or assign slots in the Inspector.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventoryManager.inventorySlots.Count)
            {
                InventorySlot slotData =
                    inventoryManager.inventorySlots[i];

                slots[i].Initialize(
                    slotData.itemData,
                    slotData.amount,
                    this
                );
            }
            else
            {
                slots[i].SetItem(null, 0);
            }
        }

        statsPanelUI?.Refresh();
    }

    private void ResolveReferences()
    {
        if (!gameObject.scene.IsValid())
        {
            return;
        }

        if (inventoryPanel == null)
        {
            Transform panelTransform = transform.Find("InventoryPanel");
            if (panelTransform != null)
            {
                inventoryPanel = panelTransform.gameObject;
            }
        }

        if (inventoryManager == null)
        {
            inventoryManager = FindAnyObjectByType<InventoryManager>();
        }

        if (equipmentManager == null)
        {
            equipmentManager = FindAnyObjectByType<EquipmentManager>();
        }

        if (tooltipUI == null)
        {
            tooltipUI = FindAnyObjectByType<TooltipUI>(FindObjectsInactive.Include);
        }

        SubscribeToInventoryManager();

        if (slots == null || slots.Length == 0)
        {
            BindSlotsFromPanel();
        }

        ResolveStatsPanel();
    }

    private void ApplyVisualStyle()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        StylePanel(inventoryPanel, InventoryUITheme.Overlay, false);

        Transform windowTransform = inventoryPanel.transform.Find("InventoryWindow");
        if (windowTransform != null)
        {
            RemoveLegacyHeader(windowTransform);
            StyleWindow(windowTransform);
        }

        Transform[] children = inventoryPanel.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child == inventoryPanel.transform)
            {
                continue;
            }

            if (child.name.Contains("StatsPanel") || child.name.Contains("EquipmentPanel"))
            {
                StylePanel(child.gameObject, InventoryUITheme.PanelSecondary, true);
            }
            else if (child.name.Contains("InventoryGrid"))
            {
                StylePanel(child.gameObject, InventoryUITheme.PanelSecondary, true);
            }
        }

        GridLayoutGroup grid = inventoryPanel.GetComponentInChildren<GridLayoutGroup>(true);
        if (grid != null)
        {
            grid.cellSize = new Vector2(92f, 92f);
            grid.spacing = new Vector2(12f, 12f);
            grid.padding = new RectOffset(16, 16, 16, 16);
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
        }
    }

    private void StyleWindow(Transform windowTransform)
    {
        RectTransform rectTransform = windowTransform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(1120f, 620f);
        }

        StylePanel(windowTransform.gameObject, InventoryUITheme.Panel, true);
    }

    private void RemoveLegacyHeader(Transform windowTransform)
    {
        DestroyChildIfPresent(windowTransform, "InventoryTitle");
        DestroyChildIfPresent(windowTransform, "InventorySubtitle");
    }

    private void DestroyChildIfPresent(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(child.gameObject);
        }
        else
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void StylePanel(GameObject panel, Color color, bool decorate)
    {
        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }

        if (!decorate)
        {
            return;
        }

        InventoryUITheme.EnsureOutline(panel, InventoryUITheme.Border, new Vector2(2f, -2f));
        InventoryUITheme.EnsureShadow(panel, new Color(0f, 0f, 0f, 0.38f), new Vector2(7f, -7f));
    }

    private void BindSlotsFromPanel()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        Transform[] children = inventoryPanel.GetComponentsInChildren<Transform>(true);
        InventorySlotUI[] orderedSlots = new InventorySlotUI[children.Length];
        int foundCount = 0;

        foreach (Transform child in children)
        {
            if (!TryGetSlotNumber(child.name, out int slotNumber))
            {
                continue;
            }

            InventorySlotUI slot = child.GetComponent<InventorySlotUI>();
            if (slot == null)
            {
                slot = child.gameObject.AddComponent<InventorySlotUI>();
            }

            slot.EnsureReferences();

            int index = slotNumber - 1;
            if (index >= orderedSlots.Length)
            {
                continue;
            }

            orderedSlots[index] = slot;
            foundCount++;
        }

        slots = new InventorySlotUI[foundCount];
        int writeIndex = 0;
        for (int i = 0; i < orderedSlots.Length; i++)
        {
            if (orderedSlots[i] == null)
            {
                continue;
            }

            slots[writeIndex] = orderedSlots[i];
            writeIndex++;
        }
    }

    private bool TryGetSlotNumber(string objectName, out int slotNumber)
    {
        slotNumber = 0;

        if (!objectName.StartsWith("Slot"))
        {
            return false;
        }

        return int.TryParse(objectName.Substring(4), out slotNumber);
    }

    private void ResolveStatsPanel()
    {
        if (statsPanelUI != null || inventoryPanel == null)
        {
            return;
        }

        Transform[] children = inventoryPanel.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name != "StatsPanel")
            {
                continue;
            }

            statsPanelUI = child.GetComponent<StatsPanelUI>();
            if (statsPanelUI == null)
            {
                statsPanelUI = child.gameObject.AddComponent<StatsPanelUI>();
            }

            statsPanelUI.Refresh();
            return;
        }
    }

    private void SubscribeToInventoryManager()
    {
        if (!isActiveAndEnabled || inventoryManager == null || subscribedInventoryManager == inventoryManager)
        {
            return;
        }

        if (subscribedInventoryManager != null)
        {
            subscribedInventoryManager.InventoryChanged -= Refresh;
        }

        subscribedInventoryManager = inventoryManager;
        subscribedInventoryManager.InventoryChanged += Refresh;
    }

    public void OnItemClicked(ItemData item)
    {
        ResolveReferences();

        if (TryUseInventoryItem(item))
        {
            return;
        }

        if (item is EquipmentData equipment)
        {
            if (equipmentManager == null)
            {
                Debug.LogWarning("[InventoryUI] Missing EquipmentManager. Add EquipmentManager to the scene or assign it in the Inspector.");
                return;
            }

            if (inventoryManager == null)
            {
                Debug.LogWarning("[InventoryUI] Missing InventoryManager. Add InventoryManager to the scene or assign it in the Inspector.");
                return;
            }

            if (!equipmentManager.CanEquip(equipment.SlotType))
            {
                Debug.LogWarning($"[InventoryUI] Cannot equip item slot {equipment.SlotType}.");
                return;
            }

            // If the clicked equipment is the same instance already equipped, do nothing.
            EquipmentData currentlyEquipped = equipmentManager.GetEquippedEquipment(equipment.SlotType);
            if (currentlyEquipped == equipment)
            {
                Debug.Log($"[InventoryUI] {equipment.itemName} is already equipped.");
                return;
            }

            if (!inventoryManager.RemoveItem(equipment))
            {
                Debug.LogWarning($"[InventoryUI] Cannot equip {equipment.itemName} because it was not found in the inventory.");
                Refresh();
                return;
            }

            EquipmentData previousEquipment = equipmentManager.Equip(equipment);
            if (previousEquipment != null && previousEquipment != equipment)
            {
                inventoryManager.AddItem(previousEquipment);
            }

            Refresh();
        }
    }

    public void OnItemRightClicked(ItemData item)
    {
        ResolveReferences();

        TryUseInventoryItem(item);
    }

    private bool TryUseInventoryItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        if (inventoryManager == null)
        {
            Debug.LogWarning("[InventoryUI] Missing InventoryManager. Add InventoryManager to the scene or assign it in the Inspector.");
            return false;
        }

        if (!CanUseFromInventory(item))
        {
            return false;
        }

        if (inventoryManager.UseItem(item))
        {
            Refresh();
            return true;
        }

        return false;
    }

    private bool CanUseFromInventory(ItemData item)
    {
        return item is ConsumableItemData || item.itemType == ItemType.Potion;
    }

    public void OnItemHovered(ItemData item, int amount = 1)
    {
        ResolveReferences();

        if (tooltipUI == null || item == null)
        {
            return;
        }

        tooltipUI.Show(item, amount);
    }

    public void OnItemHoverExit(ItemData item)
    {
        ResolveReferences();

        if (tooltipUI == null)
        {
            return;
        }

        tooltipUI.Hide();
    }
}
