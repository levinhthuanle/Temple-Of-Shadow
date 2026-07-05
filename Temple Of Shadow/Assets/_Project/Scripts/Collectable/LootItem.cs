using UnityEngine;

public class LootItem : MonoBehaviour
{
    public ItemData itemData;
    private InventoryManager inventoryManager;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

    }

    public void Initialize(ItemData item)
    {
        itemData = item;
        spriteRenderer.sprite = item.icon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        inventoryManager.AddItem(itemData);

        Destroy(gameObject);
    }
}