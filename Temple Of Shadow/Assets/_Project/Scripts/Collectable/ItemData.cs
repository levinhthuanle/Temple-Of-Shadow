
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

    private void Awake()
    {
        if (itemType == ItemType.Potion )
        {
           stackable = true;
        }
        else
        {
            stackable = false;
        }
    }
}

