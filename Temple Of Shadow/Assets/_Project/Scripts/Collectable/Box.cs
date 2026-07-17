using UnityEngine;

public class Box : MonoBehaviour, IDamageable
{
    [Header("Box Stats")]
    [SerializeField] private int maxHp = 3;
    private int currentHp;

    [Header("Drop")]
    [SerializeField] private EnemyDrop enemyDrop;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHp}");

        if (currentHp <= 0)
        {
            BreakBox();
        }
    }

    private void BreakBox()
    {
        Debug.Log($"{gameObject.name} is broken!");

        enemyDrop.DropCoins();
        Destroy(gameObject);
    }
}