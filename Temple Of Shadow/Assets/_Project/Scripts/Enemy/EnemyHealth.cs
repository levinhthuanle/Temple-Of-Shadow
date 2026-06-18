using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Animator animator;
    private EnemyAI enemyAI;
    private EnemyAttack enemyAttack;

    [SerializeField] private int maxHp = 5;
    [SerializeField] private float destroyDelay = 1f;
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private DamagePopupSpawner damagePopupSpawner;

    private int currentHp;
    private bool isDeadOrNot = false;

    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int Die = Animator.StringToHash("die");
    private static readonly int IsDead = Animator.StringToHash("isDead");

    private void Awake()
    {
        currentHp = maxHp;

        animator = GetComponentInChildren<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    private void Start()
    {
        healthBar.UpdateHealthBar(currentHp, maxHp);
    }

    //---------------------------UTILITY METHODS---------------------------//
    public void TakeDamage(int damage)
    {
        if (isDeadOrNot) return;

        currentHp -= damage;

        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHp}");
        healthBar.UpdateHealthBar(currentHp, maxHp);
        damagePopupSpawner.ShowDamage(damage);

        if (currentHp <= 0)
        {
            DieEnemy();
            return;
        }





        animator.SetTrigger(Hurt);

        if (enemyAI != null)
        {
            enemyAI.SetCanMove(false);
        }

        if (enemyAttack != null)
        {
            enemyAttack.ResetAttackState();
        }
    }

    public void FinishHurt()
    {
        if (enemyAI != null && !isDeadOrNot)
        {
            enemyAI.SetCanMove(true);
        }
    }

    private void DieEnemy()
    {
        isDeadOrNot = true;

        animator.SetBool(IsDead, true);
        animator.SetTrigger(Die);

        if (enemyAI != null)
            enemyAI.enabled = false;

        if (enemyAttack != null)
            enemyAttack.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }

    public bool IsDeadYet()
    {
        return isDeadOrNot;
    }
}