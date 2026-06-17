using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 5;

    private int currentHp;
    private Animator animator;

    private static readonly int hurt = Animator.StringToHash("Hurt");

    private void Awake()
    {
        currentHp = maxHp;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;   


        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHp}");
        Hurt();
        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Hurt()
    {
        animator.SetTrigger(hurt);
    }
}