using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;

    [Header("Invincible")]
    [SerializeField] private float invincibleTime = 0.5f;

    private int currentHp;
    private bool isInvincible;
    private bool isDead;

    private Animator animator;
    private PlayerController playerController;
    private PlayerCombat playerCombat;

    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int Die = Animator.StringToHash("die");

    private void Awake()
    {
        currentHp = maxHp;

        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        Debug.Log($"Player took {damage} damage. HP left: {currentHp}");

        if (playerCombat != null)
        {
            playerCombat.ResetCombatState();
        }

        if (currentHp > 0)
        {
            HurtPlayer();
        }
        else
        {
            DiePlayer();
        }
    }

    private void HurtPlayer()
    {
        isInvincible = true;

        if (animator != null)
        {
            animator.SetTrigger(Hurt);
        }

        if (playerController != null)
        {
            playerController.SetCanMove(false);
        }

        StartCoroutine(InvincibleRoutine());
    }

    private IEnumerator InvincibleRoutine()
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    private void DiePlayer()
    {
        isDead = true;

        Debug.Log("Player died");

        if (playerController != null)
        {
            playerController.SetCanMove(false);
        }

        if (playerCombat != null)
        {
            playerCombat.ResetCombatState();
        }

        if (animator != null)
        {
            animator.SetTrigger(Die);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Gọi bằng Animation Event ở frame cuối animation Hurt
    public void FinishHurt()
    {
        if (isDead) return;

        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }

    public int GetCurrentHp()
    {
        return currentHp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }
}