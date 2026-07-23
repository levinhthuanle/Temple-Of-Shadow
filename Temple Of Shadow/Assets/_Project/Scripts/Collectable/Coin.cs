using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;

    [Header("Idle Animation")]
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Pickup Effect")]
    [SerializeField] private bool playPickupEffect = true;
    [SerializeField] private float effectDuration = 0.3f;
    [SerializeField] private float popScale = 1.5f;
    [SerializeField] private float floatHeight = 0.6f;

    private bool isCollected;
    private Transform visual;

    private void Awake()
    {
        visual = transform.childCount > 0 ? transform.GetChild(0) : transform;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Coins are collectibles only: every collider is a trigger, never a solid obstacle.
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.isTrigger = true;
        }
    }

    private void Update()
    {
        if (isCollected) return;

        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isCollected) return;

        isCollected = true;

        PlayerWallet wallet = other.GetComponent<PlayerWallet>();
        if (wallet != null)
        {
            wallet.AddGold(value);
        }

        SoundManager.Instance?.PlaySFX("coin_pickup");

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
        // Disable collider and physics so it can't be interacted with again
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Find the visual (first child with SpriteRenderer)
        Vector3 startPos = transform.position;
        Vector3 startScale = visual.localScale;
        Vector3 popScaleVec = startScale * popScale;
        float elapsed = 0f;

        // Phase 1: quick pop
        float popDuration = effectDuration * 0.25f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            visual.localScale = Vector3.Lerp(startScale, popScaleVec, t);
            yield return null;
        }

        // Phase 2: float up & fade out
        float fadeDuration = effectDuration - popDuration;
        elapsed = 0f;

        // Get all SpriteRenderers (coin has nested structure)
        SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Float upward
            transform.position = startPos + Vector3.up * (floatHeight * t);

            // Fade all sprite renderers
            foreach (SpriteRenderer r in allRenderers)
            {
                Color c = r.color;
                c.a = 1f - t;
                r.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
