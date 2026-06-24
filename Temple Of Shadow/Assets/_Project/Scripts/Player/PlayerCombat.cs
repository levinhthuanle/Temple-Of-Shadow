using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private PlayerStats playerStats;

    [Header("Attack Point")]
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float slashRadius = 0.6f;
    [SerializeField] private float kickRadius = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    private float attackInterval = 1f;

    [Header("Throw")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject projectilePrefab;

    private bool isAttacking;
    private float nextAttackTime;

    private static readonly int Slash = Animator.StringToHash("slash");
    private static readonly int Throw = Animator.StringToHash("throw");
    private static readonly int Kick = Animator.StringToHash("kick");
    

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        
        playerController = GetComponent<PlayerController>();

        playerStats = GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged += ApplyStats;
        }

        ApplyStats();
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged -= ApplyStats;
        }
    }

    private void Update()
    {
        // Chặn tấn công nếu đang bị thương (CanMove = false) hoặc đã đang tấn công
        if (playerController == null || isAttacking || !playerController.CanMove || Time.time < nextAttackTime) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            DoSlash();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            DoThrow();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            DoKick();
        }
    }

    private void DoSlash()
    {
        BeginAttack();
        animator.SetTrigger(Slash);
        SoundManager.Instance?.PlaySFX("attack");
    }

    private void DoThrow()
    {
        BeginAttack();
        animator.SetTrigger(Throw);
        SoundManager.Instance?.PlaySFX("attack");
    }

    private void DoKick()
    {
        BeginAttack();
        animator.SetTrigger(Kick);
        SoundManager.Instance?.PlaySFX("attack");
    }

    // Gọi bằng Animation Event trong animation Slashing
    public void SlashHit()
    {
        HitEnemies(slashRadius, GetCurrentDamage());
    }

    // Gọi bằng Animation Event trong animation Kicking
    public void KickHit()
    {
        Debug.Log("KickHit CALLED");
        HitEnemies(kickRadius, GetCurrentDamage());
    }

    // Gọi bằng Animation Event trong animation Throwing
    public void ThrowProjectile()
    {

        Debug.Log("ThrowProjectile CALLED");

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is NULL");
            return;
        }

        if (throwPoint == null)
        {
            Debug.LogError("Throw Point is NULL");
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        Debug.Log("Projectile created: " + projectile.name);

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript == null)
        {
            Debug.LogError("Projectile prefab does not have Projectile.cs");
            return;
        }

        projectileScript.Setup(playerController.FacingDirection);
    }

    // Gọi bằng Animation Event ở frame cuối mỗi animation attack
    public void FinishAttack()
    {
        isAttacking = false;

        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
    }

    // Dùng để reset trạng thái đánh khi bị ngắt (như bị nhận sát thương)
    public void ResetCombatState()
    {
        isAttacking = false;
        nextAttackTime = 0f;

        if (animator != null)
        {
            animator.ResetTrigger(Slash);
            animator.ResetTrigger(Throw);
            animator.ResetTrigger(Kick);
        }
    }

    private void HitEnemies(float radius, int damage)
    {
        if (attackPoint == null)
        {
            return;
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            radius,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void BeginAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackInterval;

        if (playerController != null)
        {
            playerController.SetCanMove(false);
        }
    }

    private void ApplyStats()
    {
        if (playerStats == null)
        {
            return;
        }

        attackInterval = Mathf.Max(0.01f, playerStats.AttackInterval);
    }

    private int GetCurrentDamage()
    {
        return playerStats != null ? playerStats.Damage : 0;
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, slashRadius);
        Gizmos.DrawWireSphere(attackPoint.position, kickRadius);
    }
}