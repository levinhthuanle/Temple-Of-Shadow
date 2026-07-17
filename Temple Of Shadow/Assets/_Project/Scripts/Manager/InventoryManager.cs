using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public event System.Action InventoryChanged;

    public List<InventorySlot> inventorySlots = new();

    [SerializeField] private ItemData testSword;
    [SerializeField] private ItemData testPotion;
    [SerializeField] private GameObject itemUser;


    private int maxSlots = 16;

    public int MaxSlots => maxSlots;

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

        if (Input.GetKeyDown(KeyCode.H))
        {
            UseFirstHealthPotion();
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

    public void ReplaceInventory(IEnumerable<InventorySlot> newSlots)
    {
        inventorySlots.Clear();

        if (newSlots != null)
        {
            foreach (InventorySlot slot in newSlots)
            {
                if (slot == null || slot.itemData == null || slot.amount <= 0 || inventorySlots.Count >= maxSlots)
                {
                    continue;
                }

                inventorySlots.Add(new InventorySlot
                {
                    itemData = slot.itemData,
                    amount = slot.amount
                });
            }
        }

        InventoryChanged?.Invoke();
    }

    public bool UseItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        if (item is ConsumableItemData consumable)
        {
            GameObject user = ResolveItemUser();

            if (!consumable.CanUse(user) || !consumable.Use(user))
            {
                return false;
            }

            return RemoveItem(item);
        }

        if (item.itemType == ItemType.Potion)
        {
            return UseLegacyHealthPotion(item);
        }

        Debug.LogWarning($"[InventoryManager] {item.itemName} is not a consumable item.");
        return false;
    }

    public bool UseFirstHealthPotion()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot.itemData == null)
            {
                continue;
            }

            if (slot.itemData is HealthPotionData || slot.itemData.itemType == ItemType.Potion)
            {
                return UseItem(slot.itemData);
            }
        }

        Debug.Log("[InventoryManager] No health potion found in inventory.");
        return false;
    }

    private bool UseLegacyHealthPotion(ItemData item)
    {
        GameObject user = ResolveItemUser();
        PlayerHealth playerHealth = ResolvePlayerHealth(user);

        if (playerHealth == null)
        {
            Debug.LogWarning($"[InventoryManager] Cannot use {item.itemName}: missing PlayerHealth target.");
            return false;
        }

        if (playerHealth.GetCurrentHp() >= playerHealth.GetMaxHp())
        {
            Debug.Log($"[InventoryManager] {item.itemName} was not consumed because HP is already full.");
            return false;
        }

        playerHealth.Heal(item.healAmount);
        return RemoveItem(item);
    }

    public bool UseFirstConsumable<TConsumable>() where TConsumable : ConsumableItemData
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot.itemData is TConsumable)
            {
                return UseItem(slot.itemData);
            }
        }

        Debug.Log($"[InventoryManager] No {typeof(TConsumable).Name} found in inventory.");
        return false;
    }

    private GameObject ResolveItemUser()
    {
        if (itemUser != null)
        {
            return itemUser;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            itemUser = player;
            return itemUser;
        }

        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            itemUser = playerHealth.gameObject;
        }

        return itemUser;
    }

    private PlayerHealth ResolvePlayerHealth(GameObject user)
    {
        if (user != null)
        {
            PlayerHealth playerHealth = user.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                return playerHealth;
            }

            playerHealth = user.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                return playerHealth;
            }
        }

        return FindAnyObjectByType<PlayerHealth>();
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
