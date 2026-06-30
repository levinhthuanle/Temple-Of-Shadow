using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    public InventoryManager inventoryManager;

    public InventorySlotUI[] slots;

    private bool isOpen;
    private InventoryManager subscribedInventoryManager;
    private StatsPanelUI statsPanelUI;

    private void Awake()
    {
        ResolveReferences();
        Refresh();
    }

    private void OnEnable()
    {
        ResolveReferences();

        SubscribeToInventoryManager();
    }

    private void OnDisable()
    {
        if (subscribedInventoryManager != null)
        {
            subscribedInventoryManager.InventoryChanged -= Refresh;
            subscribedInventoryManager = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
            Refresh();
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;

        inventoryPanel.SetActive(isOpen);
    }

    public void Refresh()
    {
        ResolveReferences();

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

                slots[i].SetItem(
                    slotData.itemData,
                    slotData.amount
                );
            }
            else
            {
                slots[i].SetItem(null, 0);
            }
        }

        Debug.Log($"[InventoryUI] Refreshed {inventoryManager.inventorySlots.Count} data slots into {slots.Length} UI slots.");
    }

    private void ResolveReferences()
    {
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

        SubscribeToInventoryManager();

        if (slots == null || slots.Length == 0)
        {
            BindSlotsFromPanel();
        }

        ResolveStatsPanel();
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
}
