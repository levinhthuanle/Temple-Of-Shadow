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

    public void AddItem(ItemData item)
    {
        if (item == null)
            return;

        if (item.stackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.itemData == item)
                {
                    slot.amount++;
                    InventoryChanged?.Invoke();
                    return;

                }
            }
        }

        if (inventorySlots.Count >= maxSlots)
        {
            Debug.Log("Inventory Full");
            return;
        }

        InventorySlot newSlot = new InventorySlot();

        newSlot.itemData = item;
        newSlot.amount = 1;

        inventorySlots.Add(newSlot);
        InventoryChanged?.Invoke();

        
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
