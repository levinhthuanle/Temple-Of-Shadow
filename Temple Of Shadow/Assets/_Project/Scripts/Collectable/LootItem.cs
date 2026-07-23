using System.Collections;
using UnityEngine;

public class LootItem : MonoBehaviour
{
    public ItemData itemData;
    private InventoryManager inventoryManager;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Pickup Effect")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private bool playPickupEffect = true;
    [SerializeField] private float effectDuration = 0.4f;
    [SerializeField] private float popScale = 1.5f;
    [SerializeField] private float floatHeight = 1f;

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

        // Play pickup sound (one-shot, survives object destruction)
        if (pickupSound != null)
        {
            SoundManager.Instance?.PlaySFX(pickupSound);
        }

        if (playPickupEffect)
        {
            StartCoroutine(PickupEffectRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator PickupEffectRoutine()
    {
        // Disable the collider so it can't be picked up again
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        Vector3 popScaleVec = startScale * popScale;
        float elapsed = 0f;

        // Phase 1: quick pop (scale up)
        float popDuration = effectDuration * 0.25f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            transform.localScale = Vector3.Lerp(startScale, popScaleVec, t);
            yield return null;
        }

        // Phase 2: float up & fade out
        float fadeDuration = effectDuration - popDuration;
        elapsed = 0f;
        Color spriteColor = spriteRenderer.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Float upward
            transform.position = startPos + Vector3.up * (floatHeight * t);

            // Fade out
            spriteColor.a = 1f - t;
            spriteRenderer.color = spriteColor;

            yield return null;
        }

        Destroy(gameObject);
    }
}