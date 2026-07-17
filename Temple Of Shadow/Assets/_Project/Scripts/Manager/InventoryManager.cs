using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public event System.Action InventoryChanged;

    public List<InventorySlot> inventorySlots = new();

    [SerializeField] private ItemData testSword;
    [SerializeField] private ItemData testPotion;


    private int maxSlots = 16;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddItem(testSword);
            PrintInventory();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            AddItem(testPotion);
            PrintInventory();
        }


    }

    public bool AddItem(ItemData item)
    {
        if (item == null)
            return false;

        if (item.stackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.itemData == item)
                {
                    slot.amount++;
                    InventoryChanged?.Invoke();
                    return true;

                }
            }
        }

        if (inventorySlots.Count >= maxSlots)
        {
            Debug.Log("Inventory Full");
            return false;
        }

        InventorySlot newSlot = new InventorySlot();

        newSlot.itemData = item;
        newSlot.amount = 1;

        inventorySlots.Add(newSlot);
        InventoryChanged?.Invoke();
        return true;
    }

    public bool CanAddItem(ItemData item)
    {
        if (item == null)
            return false;

        if (item.stackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.itemData == item)
                    return true;
            }
        }

        return inventorySlots.Count < maxSlots;
    }

    public bool RemoveItem(ItemData item)
    {
        if (item == null)
            return false;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot.itemData != item)
            {
                continue;
            }

            slot.amount--;
            if (slot.amount <= 0)
            {
                inventorySlots.RemoveAt(i);
            }

            InventoryChanged?.Invoke();
            return true;
        }

        return false;
    }

    public void PrintInventory()
    {
        Debug.Log("====== INVENTORY ======");
        Debug.Log("Inventory Slots: " + inventorySlots.Count + "/" + maxSlots);

        foreach (InventorySlot slot in inventorySlots)
        {
            Debug.Log(slot.itemData.itemName + " x" + slot.amount);
        }
    }
}
