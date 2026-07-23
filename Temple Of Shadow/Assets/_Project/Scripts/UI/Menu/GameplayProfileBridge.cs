using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class GameplayProfileBridge : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private EquipmentManager equipmentManager;
    private bool applyingProfile;
    private bool initialized;

    private void Awake()
    {
        ApplyProfile();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        if (initialized)
        {
            SyncProfile();
        }
    }

    private void ApplyProfile()
    {
        GameProfileData profile = GameSession.EnsureProfile();
        GameContentCatalog catalog = GameSession.Catalog;
        if (profile == null || catalog == null)
        {
            Debug.LogWarning("[GameplayProfileBridge] Missing active profile or content catalog.");
            return;
        }

        applyingProfile = true;
        CharacterMenuEntry character = catalog.GetCharacter(profile.selectedCharacterId)
            ?? catalog.GetFirstAvailableCharacter();
        GameObject player = ResolvePlayer(character);

        inventoryManager = FindFirstObjectByType<InventoryManager>();
        equipmentManager = FindFirstObjectByType<EquipmentManager>();

        if (player != null)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null && character != null)
            {
                stats.SetCharacterData(character.data);
            }

            if (equipmentManager != null)
            {
                equipmentManager.playerBonus = player.GetComponent<PlayerBonus>();
            }
        }

        ApplyInventory(profile, catalog);
        ApplyEquipment(profile, catalog);
        Subscribe();
        applyingProfile = false;
        initialized = true;
    }

    private GameObject ResolvePlayer(CharacterMenuEntry character)
    {
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
        if (character == null || character.playerPrefab == null)
        {
            return currentPlayer;
        }

        PlayerStats currentStats = currentPlayer != null ? currentPlayer.GetComponent<PlayerStats>() : null;
        if (currentStats != null && currentStats.CharacterData == character.data)
        {
            return currentPlayer;
        }

        Vector3 position = currentPlayer != null ? currentPlayer.transform.position : Vector3.zero;
        Quaternion rotation = currentPlayer != null ? currentPlayer.transform.rotation : Quaternion.identity;
        Vector3 scale = currentPlayer != null ? currentPlayer.transform.localScale : character.playerPrefab.transform.localScale;

        GameObject newPlayer = Instantiate(character.playerPrefab, position, rotation);
        newPlayer.name = "Player";
        newPlayer.tag = "Player";
        newPlayer.transform.localScale = scale;

        foreach (CinemachineCamera camera in FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None))
        {
            camera.Follow = newPlayer.transform;
        }

        if (currentPlayer != null)
        {
            currentPlayer.SetActive(false);
            Destroy(currentPlayer);
        }

        return newPlayer;
    }

    private void ApplyInventory(GameProfileData profile, GameContentCatalog catalog)
    {
        if (inventoryManager == null)
        {
            return;
        }

        List<InventorySlot> slots = new();
        foreach (InventoryEntryData entry in profile.inventory)
        {
            ItemData item = catalog.GetItem(entry.itemId);
            if (item == null || entry.amount <= 0)
            {
                continue;
            }

            slots.Add(new InventorySlot { itemData = item, amount = entry.amount });
        }

        inventoryManager.ReplaceInventory(slots);
    }

    private void ApplyEquipment(GameProfileData profile, GameContentCatalog catalog)
    {
        if (equipmentManager == null)
        {
            return;
        }

        EquipmentData sword = ResolveEquipment(profile, catalog, EquipmentSlotType.Sword);
        EquipmentData armor = ResolveEquipment(profile, catalog, EquipmentSlotType.Armor);
        EquipmentData accessory = ResolveEquipment(profile, catalog, EquipmentSlotType.Accessory);
        EquipmentData projectile = ResolveEquipment(profile, catalog, EquipmentSlotType.Projectile);
        equipmentManager.ApplyLoadout(sword, armor, accessory, projectile);
    }

    private EquipmentData ResolveEquipment(
        GameProfileData profile,
        GameContentCatalog catalog,
        EquipmentSlotType slot)
    {
        return catalog.GetItem(profile.GetEquippedItemId(slot)) as EquipmentData;
    }

    private void Subscribe()
    {
        if (inventoryManager != null)
        {
            inventoryManager.InventoryChanged += SyncProfile;
        }

        if (equipmentManager != null)
        {
            equipmentManager.EquipmentChanged += SyncProfile;
        }
    }

    private void Unsubscribe()
    {
        if (inventoryManager != null)
        {
            inventoryManager.InventoryChanged -= SyncProfile;
        }

        if (equipmentManager != null)
        {
            equipmentManager.EquipmentChanged -= SyncProfile;
        }
    }

    private void SyncProfile()
    {
        if (applyingProfile || GameSession.CurrentProfile == null)
        {
            return;
        }

        GameProfileData profile = GameSession.CurrentProfile;
        if (inventoryManager != null)
        {
            profile.inventory.Clear();
            foreach (InventorySlot slot in inventoryManager.inventorySlots)
            {
                if (slot.itemData == null || slot.amount <= 0)
                {
                    continue;
                }

                profile.inventory.Add(new InventoryEntryData
                {
                    itemId = GameContentCatalog.GetItemId(slot.itemData),
                    amount = slot.amount
                });
            }
        }

        if (equipmentManager != null)
        {
            profile.equipment.Clear();
            StoreEquipment(profile, equipmentManager.equippedSword);
            StoreEquipment(profile, equipmentManager.equippedArmor);
            StoreEquipment(profile, equipmentManager.equippedAccessory);
            StoreEquipment(profile, equipmentManager.equippedProjectile);
        }

        GameSession.Save();
    }

    private void StoreEquipment(GameProfileData profile, EquipmentData equipment)
    {
        if (equipment == null || equipment.SlotType == EquipmentSlotType.None)
        {
            return;
        }

        profile.SetEquippedItem(equipment.SlotType, GameContentCatalog.GetItemId(equipment));
    }
}
