using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Animator animator;
    private EnemyAI enemyAI;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private LayerMask playerLayer;

    private float lastAttackTime;
    private bool isAttacking;

    private static readonly int Attack = Animator.StringToHash("attack");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void TryAttack()
    {
        if (isAttacking) return;
        if (enemyAI != null && !enemyAI.CanMove) return; // Fix để không phát sinh rác logic nếu đang ăn Hurt

        if (Time.time < lastAttackTime + attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        enemyAI.SetCanMove(false);
        animator.SetTrigger(Attack);
    }

    public void DealDamage()
    {
        Collider2D player = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
        enemyAI.SetCanMove(true);
    }

    public void ResetAttackState()
    {
        isAttacking = false;
        if (animator != null)
        {
            animator.ResetTrigger(Attack);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}