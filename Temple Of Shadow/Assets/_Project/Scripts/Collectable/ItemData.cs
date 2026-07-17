
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public bool stackable;

    [TextArea]
    public string description;

    public Sprite icon;

    public ItemType itemType;

    [Header("Potion Settings")]
    [Min(1)]
    public int healAmount = 40;

    private void Awake()
    {
        ApplyItemDefaults();
    }

    private void OnValidate()
    {
        ApplyItemDefaults();
    }

    protected void ApplyItemDefaults()
    {
        if (itemType == ItemType.Potion)
        {
            stackable = true;
        }
        else
        {
            stackable = false;
        }
    }
}

