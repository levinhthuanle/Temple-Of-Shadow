using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;

    [TextArea]
    public string description;

    public Sprite icon;

    public ItemType itemType;
}
