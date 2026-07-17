using System;
using UnityEngine;

[Serializable]
public class CharacterMenuEntry
{
    public string id;
    public CharacterData data;
    public GameObject playerPrefab;
    public string role;

    [TextArea]
    public string description;

    public bool available = true;
    public string unavailableReason;

    public string Id => !string.IsNullOrWhiteSpace(id)
        ? id
        : data != null ? data.name : string.Empty;
}

[Serializable]
public class LevelMenuEntry
{
    public string id;
    public string displayName;
    public string sceneName;
    public string difficulty;

    [TextArea]
    public string description;

    public Sprite thumbnail;
    public bool unlockedByDefault;

    public string Id => !string.IsNullOrWhiteSpace(id) ? id : sceneName;
}

[Serializable]
public class StartingItemEntry
{
    public ItemData item;
    [Min(1)] public int amount = 1;
}

[CreateAssetMenu(menuName = "Game/Menu Content Catalog")]
public class GameContentCatalog : ScriptableObject
{
    private const string ResourceName = "GameContentCatalog";

    public CharacterMenuEntry[] characters = Array.Empty<CharacterMenuEntry>();
    public ItemData[] items = Array.Empty<ItemData>();
    public StartingItemEntry[] startingInventory = Array.Empty<StartingItemEntry>();
    public EquipmentData[] startingEquipment = Array.Empty<EquipmentData>();
    public LevelMenuEntry[] levels = Array.Empty<LevelMenuEntry>();

    private static GameContentCatalog cached;

    public static GameContentCatalog Load()
    {
        if (cached == null)
        {
            cached = Resources.Load<GameContentCatalog>(ResourceName);
        }

        return cached;
    }

    public CharacterMenuEntry GetCharacter(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        foreach (CharacterMenuEntry entry in characters)
        {
            if (entry != null && string.Equals(entry.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                return entry;
            }
        }

        return null;
    }

    public CharacterMenuEntry GetFirstAvailableCharacter()
    {
        foreach (CharacterMenuEntry entry in characters)
        {
            if (entry != null && entry.available && entry.data != null && entry.playerPrefab != null)
            {
                return entry;
            }
        }

        return null;
    }

    public ItemData GetItem(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        foreach (ItemData item in items)
        {
            if (item != null && string.Equals(GetItemId(item), id, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }

        return null;
    }

    public LevelMenuEntry GetLevel(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        foreach (LevelMenuEntry level in levels)
        {
            if (level != null && string.Equals(level.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                return level;
            }
        }

        return null;
    }

    public LevelMenuEntry GetLevelByScene(string sceneName)
    {
        foreach (LevelMenuEntry level in levels)
        {
            if (level != null && string.Equals(level.sceneName, sceneName, StringComparison.OrdinalIgnoreCase))
            {
                return level;
            }
        }

        return null;
    }

    public LevelMenuEntry GetFirstDefaultLevel()
    {
        foreach (LevelMenuEntry level in levels)
        {
            if (level != null && level.unlockedByDefault)
            {
                return level;
            }
        }

        return levels.Length > 0 ? levels[0] : null;
    }

    public static string GetItemId(ItemData item)
    {
        return item != null ? item.name : string.Empty;
    }
}
