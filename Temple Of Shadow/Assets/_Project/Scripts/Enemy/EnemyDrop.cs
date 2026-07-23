using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [Header("Coin Drop")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinAmount = 3;

    [Header("Spread")]
    [SerializeField] private float dropHeight = 0.5f;
    [SerializeField] private float coinSpread = 0.55f;

    [Header("Sound")]
    [SerializeField] private AudioClip dropSound;

    private float centerOffset;

    private void Awake()
    {
        Collider2D sourceCollider = GetComponent<Collider2D>();
        centerOffset = sourceCollider != null
            ? sourceCollider.bounds.center.y - transform.position.y
            : 0f;
    }

    public void DropCoins()
    {
        if (dropSound != null)
        {
            SoundManager.Instance?.PlaySFX(dropSound);
        }

        for (int i = 0; i < coinAmount; i++)
        {
            SpawnCoin(i);
        }
    }

    private void SpawnCoin(int index)
    {
        float centeredIndex = index - (coinAmount - 1) * 0.5f;
        Vector3 spawnPos = new Vector3(
            transform.position.x + centeredIndex * coinSpread,
            transform.position.y + centerOffset + dropHeight,
            transform.position.z
        );

        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }
}
