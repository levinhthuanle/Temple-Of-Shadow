using UnityEngine;

public class LootItem : MonoBehaviour
{
    public ItemData itemData;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void Initialize(ItemData item)
    {
        itemData = item;
        spriteRenderer.sprite = item.icon;
    }
}