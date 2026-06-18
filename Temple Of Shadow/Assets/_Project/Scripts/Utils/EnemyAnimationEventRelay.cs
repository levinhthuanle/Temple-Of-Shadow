using UnityEngine;

public class EnemyAnimationEventRelay : MonoBehaviour
{
    private EnemyAttack enemyAttack;
    private RangeEnemyAttack rangeEnemyAttack;
    private EnemyHealth health;

    private void Awake()
    {
        enemyAttack = GetComponentInParent<EnemyAttack>();
        health = GetComponentInParent<EnemyHealth>();
        rangeEnemyAttack = GetComponentInParent<RangeEnemyAttack>();
    }

    public void DealDamage()
    {
        if (enemyAttack != null)
        {
            Debug.Log("DealDamage called from animation event");
            enemyAttack.DealDamage();
        }
    }

    public void ShootProjectile()
    {
        if (rangeEnemyAttack != null)
        {
            Debug.Log("ShootProjectile called from animation event");
            rangeEnemyAttack.ShootProjectile();
        }
            
    }
    public void Finish()
    {
        Debug.Log("FinishAttack called from animation event");
        if (enemyAttack != null)
            enemyAttack.FinishAttack();

        if (rangeEnemyAttack != null)
            rangeEnemyAttack.FinishAttack();
    }

    public void FinishHurt()
    {
        Debug.Log("Finish hurt");
        health.FinishHurt();
    }
}