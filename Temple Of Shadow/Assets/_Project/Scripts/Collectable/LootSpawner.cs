using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField]
    private LootItem lootPrefab;

    [SerializeField]
    private ItemData testItem;

    private void Start()
    {
        LootItem loot =
            Instantiate(
                lootPrefab,
                transform.position,
                Quaternion.identity);

        loot.Initialize(testItem);
    }
}