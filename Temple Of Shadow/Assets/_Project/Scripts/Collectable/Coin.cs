using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerWallet wallet = other.GetComponent<PlayerWallet>();
        if (wallet != null)
        {
            wallet.AddGold(value);
        }

        // Played through the persistent SoundManager so it still sounds after this coin is destroyed.
        SoundManager.Instance?.PlaySFX("coin_pickup");

        Destroy(gameObject);
    }
}