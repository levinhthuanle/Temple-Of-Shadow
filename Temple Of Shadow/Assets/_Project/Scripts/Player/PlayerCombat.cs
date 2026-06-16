using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    [Header("Attack Point")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float slashRadius = 0.6f;
    [SerializeField] private float kickRadius = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Damage")]
    [SerializeField] private int slashDamage = 2;
    [SerializeField] private int kickDamage = 1;

    [Header("Throw")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject projectilePrefab;

    private bool isAttacking;

    private static readonly int Slash = Animator.StringToHash("slash");
    private static readonly int Throw = Animator.StringToHash("throw");
    private static readonly int Kick = Animator.StringToHash("kick");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (isAttacking) return;

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
        isAttacking = true;
        playerController.SetCanMove(false);
        animator.SetTrigger(Slash);
    }

    private void DoThrow()
    {
        isAttacking = true;
        playerController.SetCanMove(false);
        animator.SetTrigger(Throw);
    }

    private void DoKick()
    {
        isAttacking = true;
        playerController.SetCanMove(false);
        animator.SetTrigger(Kick);
    }

    // Gọi bằng Animation Event trong animation Slashing
    public void SlashHit()
    {
        HitEnemies(slashRadius, slashDamage);
    }

    // Gọi bằng Animation Event trong animation Kicking
    public void KickHit()
    {
        Debug.Log("KickHit CALLED");
        HitEnemies(kickRadius, kickDamage);
    }

    // Gọi bằng Animation Event trong animation Throwing
    public void ThrowProjectile()
    {
        //if (projectilePrefab == null || throwPoint == null) return;

        //GameObject projectile = Instantiate(
        //    projectilePrefab,
        //    throwPoint.position,
        //    Quaternion.identity
        //);

        //Projectile projectileScript = projectile.GetComponent<Projectile>();

        //if (projectileScript != null)
        //{
        //    projectileScript.Setup(playerController.FacingDirection);
        //}
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
        Debug.Log("FinishAttack CALLED");
        isAttacking = false;
        playerController.SetCanMove(true);
    }

    private void HitEnemies(float radius, int damage)
    {
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, slashRadius);
        Gizmos.DrawWireSphere(attackPoint.position, kickRadius);
    }
}