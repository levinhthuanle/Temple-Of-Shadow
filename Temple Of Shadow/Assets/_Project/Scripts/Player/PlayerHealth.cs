using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;

    private int currentHp;
    private Animator animator;
    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private static readonly int Hurt = Animator.StringToHash("hurt");

    private void Awake()
    {
        currentHp = maxHp;
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        Debug.Log($"Player took {damage} damage. HP left: {currentHp}");

        if (currentHp > 0)
        {
            if (animator != null)
            {
                animator.SetTrigger(Hurt);
            }

            if (playerController != null)
            {
                playerController.SetCanMove(false);
            }

            if (playerCombat != null)
            {
                playerCombat.ResetCombatState();
            }
        }
        else
        {
            Debug.Log("Player died");
            // Có thể thêm animator.SetTrigger("die"); ở đây sau này
        }
    }

    // Gọi bằng Animation Event ở frame cuối của animation hurt
    public void FinishHurt()
    {
        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
    }

    
}