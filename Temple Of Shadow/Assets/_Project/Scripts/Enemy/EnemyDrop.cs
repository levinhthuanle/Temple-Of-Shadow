using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinAmount = 3;
    [SerializeField] private float dropRadius = 0.5f;

    public void DropCoins()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-dropRadius, dropRadius),
                Random.Range(0.2f, 0.8f),
                0
            );

            Instantiate(
                coinPrefab,
                transform.position + randomOffset,
                Quaternion.identity
            );
        }
    }
}