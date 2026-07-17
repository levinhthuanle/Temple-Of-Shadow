using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class InventoryEntryData
{
    public string itemId;
    public int amount;
}

[Serializable]
public class EquipmentEntryData
{
    public EquipmentSlotType slot;
    public string itemId;
}

[Serializable]
public class GameProfileData
{
    public int version = 1;
    public int slotIndex;
    public string createdUtc;
    public string lastPlayedUtc;
    public string selectedCharacterId;
    public string selectedLevelId;
    public List<string> unlockedLevelIds = new();
    public List<InventoryEntryData> inventory = new();
    public List<EquipmentEntryData> equipment = new();

    public int GetItemAmount(string itemId)
    {
        InventoryEntryData entry = FindInventoryEntry(itemId);
        return entry != null ? entry.amount : 0;
    }

    public void AddItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId) || amount <= 0)
        {
            return;
        }

        InventoryEntryData entry = FindInventoryEntry(itemId);
        if (entry == null)
        {
            inventory.Add(new InventoryEntryData { itemId = itemId, amount = amount });
            return;
        }

        entry.amount += amount;
    }

    public bool RemoveItem(string itemId, int amount = 1)
    {
        InventoryEntryData entry = FindInventoryEntry(itemId);
        if (entry == null || amount <= 0 || entry.amount < amount)
        {
            return false;
        }

        entry.amount -= amount;
        if (entry.amount <= 0)
        {
            inventory.Remove(entry);
        }

        return true;
    }

    public string GetEquippedItemId(EquipmentSlotType slot)
    {
        EquipmentEntryData entry = equipment.Find(candidate => candidate.slot == slot);
        return entry != null ? entry.itemId : string.Empty;
    }

    public void SetEquippedItem(EquipmentSlotType slot, string itemId)
    {
        EquipmentEntryData entry = equipment.Find(candidate => candidate.slot == slot);
        if (string.IsNullOrWhiteSpace(itemId))
        {
            if (entry != null)
            {
                equipment.Remove(entry);
            }

            return;
        }

        if (entry == null)
        {
            equipment.Add(new EquipmentEntryData { slot = slot, itemId = itemId });
        }
        else
        {
            entry.itemId = itemId;
        }
    }

    public bool IsLevelUnlocked(string levelId)
    {
        return unlockedLevelIds.Exists(id => string.Equals(id, levelId, StringComparison.OrdinalIgnoreCase));
    }

    private InventoryEntryData FindInventoryEntry(string itemId)
    {
        return inventory.Find(entry => string.Equals(entry.itemId, itemId, StringComparison.OrdinalIgnoreCase));
    }
}

public static class GameProfileStore
{
    private const string FilePrefix = "temple_profile_";

    public static bool Exists(int slotIndex)
    {
        return File.Exists(GetPath(slotIndex));
    }

    public static GameProfileData TryLoad(int slotIndex)
    {
        string path = GetPath(slotIndex);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameProfileData profile = JsonUtility.FromJson<GameProfileData>(json);
            EnsureCollections(profile);
            return profile;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[GameProfileStore] Could not load slot {slotIndex}: {exception.Message}");
            return null;
        }
    }

    public static GameProfileData LoadOrCreate(int slotIndex, GameContentCatalog catalog)
    {
        GameProfileData profile = TryLoad(slotIndex);
        return profile ?? CreateDefault(slotIndex, catalog);
    }

    public static void Save(GameProfileData profile)
    {
        if (profile == null)
        {
            return;
        }

        try
        {
            EnsureCollections(profile);
            profile.lastPlayedUtc = DateTime.UtcNow.ToString("O");
            File.WriteAllText(GetPath(profile.slotIndex), JsonUtility.ToJson(profile, true));
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[GameProfileStore] Could not save slot {profile.slotIndex}: {exception.Message}");
        }
    }

    private static GameProfileData CreateDefault(int slotIndex, GameContentCatalog catalog)
    {
        string now = DateTime.UtcNow.ToString("O");
        GameProfileData profile = new()
        {
            slotIndex = slotIndex,
            createdUtc = now,
            lastPlayedUtc = now
        };

        if (catalog == null)
        {
            return profile;
        }

        CharacterMenuEntry character = catalog.GetFirstAvailableCharacter();
        LevelMenuEntry level = catalog.GetFirstDefaultLevel();
        profile.selectedCharacterId = character != null ? character.Id : string.Empty;
        profile.selectedLevelId = level != null ? level.Id : string.Empty;

        foreach (LevelMenuEntry levelEntry in catalog.levels)
        {
            if (levelEntry != null && levelEntry.unlockedByDefault)
            {
                profile.unlockedLevelIds.Add(levelEntry.Id);
            }
        }

        foreach (StartingItemEntry startingItem in catalog.startingInventory)
        {
            if (startingItem != null && startingItem.item != null)
            {
                profile.AddItem(GameContentCatalog.GetItemId(startingItem.item), Mathf.Max(1, startingItem.amount));
            }
        }

        foreach (EquipmentData equipment in catalog.startingEquipment)
        {
            if (equipment != null && equipment.SlotType != EquipmentSlotType.None)
            {
                profile.SetEquippedItem(equipment.SlotType, GameContentCatalog.GetItemId(equipment));
            }
        }

        return profile;
    }

    private static void EnsureCollections(GameProfileData profile)
    {
        if (profile == null)
        {
            return;
        }

        profile.inventory ??= new List<InventoryEntryData>();
        profile.equipment ??= new List<EquipmentEntryData>();
        profile.unlockedLevelIds ??= new List<string>();
    }

    private static string GetPath(int slotIndex)
    {
        int safeSlot = Mathf.Clamp(slotIndex, 1, 3);
        return Path.Combine(Application.persistentDataPath, $"{FilePrefix}{safeSlot}.json");
    }
}

public static class GameSession
{
    public static GameProfileData CurrentProfile { get; private set; }
    public static GameContentCatalog Catalog { get; private set; }

    public static GameProfileData SelectSlot(int slotIndex)
    {
        Catalog = GameContentCatalog.Load();
        CurrentProfile = GameProfileStore.LoadOrCreate(slotIndex, Catalog);
        GameProfileStore.Save(CurrentProfile);
        return CurrentProfile;
    }

    public static GameProfileData EnsureProfile()
    {
        if (CurrentProfile == null)
        {
            SelectSlot(1);
        }

        return CurrentProfile;
    }

    public static void Save()
    {
        GameProfileStore.Save(CurrentProfile);
    }
}
