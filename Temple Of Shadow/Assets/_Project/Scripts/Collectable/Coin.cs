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

        Destroy(gameObject);
    }
}