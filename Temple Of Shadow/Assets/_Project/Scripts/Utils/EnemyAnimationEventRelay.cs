using UnityEngine;

public class EnemyAnimationEventRelay : MonoBehaviour
{
    private EnemyAttack enemyAttack;
    private EnemyHealth health;

    private void Awake()
    {
        enemyAttack = GetComponentInParent<EnemyAttack>();
        health = GetComponentInParent<EnemyHealth>();
    }

    public void DealDamage()
    {
        Debug.Log("DealDamage called from animation event");
        enemyAttack.DealDamage();
    }

    public void Finish()
    {
        Debug.Log("FinishAttack called from animation event");
        enemyAttack.FinishAttack();
    }

    public void FinishHurt()
    {
        Debug.Log("Finish hurt");
        health.FinishHurt();
    }
}