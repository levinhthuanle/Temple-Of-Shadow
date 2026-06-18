using UnityEngine;

public class RangeEnemyAttack : MonoBehaviour
{
    private Animator animator;
    private RangeEnemyAI ai;

    [Header("Attack")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackCooldown = 1.5f;

    private bool isAttacking;
    private float lastAttackTime;

    private static readonly int Attack = Animator.StringToHash("throw");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        ai = GetComponent<RangeEnemyAI>();
    }

    public void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        ai.SetCanMove(false);
        animator.SetTrigger(Attack);
    }

    public void ShootProjectile()
    {
        if (projectilePrefab == null || shootPoint == null)
        {
            Debug.LogWarning("Missing projectile prefab or shoot point.");
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            enemyProjectile.Setup(ai.GetFacingDirection());
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
        ai.SetCanMove(true);
    }
}