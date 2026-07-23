using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BreakableThinWall : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int maxHealth = 3;

    private int currentHealth;
    private bool isBroken;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isBroken || damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            isBroken = true;
            Destroy(gameObject);
        }
    }
}
