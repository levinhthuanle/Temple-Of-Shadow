using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerBonus playerBonusStats;
    [SerializeField] private int maxHp = 10;
    [SerializeField] private int armor = 0;

    [Header("Invincible")]
    [SerializeField] private float invincibleTime = 0.5f;
    [SerializeField] private DamagePopupSpawner damagePopupSpawner;

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
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();

        playerBonusStats = GetComponent<PlayerBonus>();
        playerStats = GetComponent<PlayerStats>();
        maxHp = playerStats.MaxHP + playerBonusStats.bonusHP;
        currentHp = maxHp;
        armor = playerStats.Armor + playerBonusStats.bonusArmor;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHp -= Mathf.Max(1, damage - armor);
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        Debug.Log($"Player took {damage} damage. HP left: {currentHp}");
        damagePopupSpawner.ShowDamage(damage);

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
        SoundManager.Instance?.PlaySFX("hurt");

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
        SoundManager.Instance?.PlaySFX("player_death");

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