using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Character Data")]
    [SerializeField] private CharacterData characterData;

    [SerializeField] private PlayerBonus playerBonusStats;

    public event Action StatsChanged;

    public CharacterData CharacterData => characterData;

    public int MaxHP => GetBaseMaxHP() + GetBonusHP();

    public int Damage => GetBaseDamage() + GetBonusDamage();

    public int Armor => GetBaseArmor() + GetBonusArmor();

    public float MoveSpeed => GetBaseMoveSpeed() + GetBonusMoveSpeed();

    public float AttackSpeed => Mathf.Max(0.01f, GetBaseAttackSpeed() + GetBonusAttackSpeed());

    public float AttackInterval => 1f / AttackSpeed;

    public float JumpForce => GetBaseJumpForce() + GetBonusJumpForce();

    public int BonusJumpCount => GetBonusJumpCount();

    private CharacterData cachedCharacterData;
    private int cachedBonusHP;
    private int cachedBonusDamage;
    private int cachedBonusArmor;
    private float cachedBonusMoveSpeed;
    private float cachedBonusAttackSpeed;
    private float cachedBonusJumpForce;
    private int cachedBonusJumpCount;
    private bool hasCachedStats;

    private void Awake()
    {
        ResolveBonusStats();
        CacheStats();
    }

    private void Update()
    {
        if (!HasStatsChanged())
        {
            return;
        }

        CacheStats();
        StatsChanged?.Invoke();
    }

    private void Start()
    {
        if (characterData == null)
        {
            Debug.LogWarning("PlayerStats has no CharacterData assigned.");
            return;
        }

        Debug.Log("Character: " + characterData.characterName);
        Debug.Log("HP: " + MaxHP);
        Debug.Log("Damage: " + Damage);
    }

    public void SetCharacterData(CharacterData newCharacterData)
    {
        characterData = newCharacterData;
        CacheStats();
        StatsChanged?.Invoke();
    }

    public void SetBonusSource(PlayerBonus newBonusStats)
    {
        playerBonusStats = newBonusStats;
        CacheStats();
        StatsChanged?.Invoke();
    }

    public void RefreshStats()
    {
        CacheStats();
        StatsChanged?.Invoke();
    }

    private PlayerBonus ResolveBonusStats()
    {
        if (playerBonusStats == null)
        {
            playerBonusStats = GetComponent<PlayerBonus>();
        }

        return playerBonusStats;
    }

    private void CacheStats()
    {
        PlayerBonus bonusStats = ResolveBonusStats();

        cachedCharacterData = characterData;
        cachedBonusHP = bonusStats != null ? bonusStats.bonusHP : 0;
        cachedBonusDamage = bonusStats != null ? bonusStats.bonusDamage : 0;
        cachedBonusArmor = bonusStats != null ? bonusStats.bonusArmor : 0;
        cachedBonusMoveSpeed = bonusStats != null ? bonusStats.bonusMoveSpeed : 0f;
        cachedBonusAttackSpeed = bonusStats != null ? bonusStats.bonusAttackSpeed : 0f;
        cachedBonusJumpForce = bonusStats != null ? bonusStats.bonusJumpForce : 0f;
        cachedBonusJumpCount = bonusStats != null ? bonusStats.bonusJumpCount : 0;
        hasCachedStats = true;
    }

    private bool HasStatsChanged()
    {
        PlayerBonus bonusStats = ResolveBonusStats();

        if (!hasCachedStats)
        {
            return true;
        }

        return cachedCharacterData != characterData
            || cachedBonusHP != GetBonusHP(bonusStats)
            || cachedBonusDamage != GetBonusDamage(bonusStats)
            || cachedBonusArmor != GetBonusArmor(bonusStats)
            || !Mathf.Approximately(cachedBonusMoveSpeed, GetBonusMoveSpeed(bonusStats))
            || !Mathf.Approximately(cachedBonusAttackSpeed, GetBonusAttackSpeed(bonusStats))
            || !Mathf.Approximately(cachedBonusJumpForce, GetBonusJumpForce(bonusStats))
            || cachedBonusJumpCount != GetBonusJumpCount(bonusStats);
    }

    private int GetBaseMaxHP()
    {
        return characterData != null ? characterData.maxHp : 0;
    }

    private int GetBaseDamage()
    {
        return characterData != null ? characterData.damage : 0;
    }

    private int GetBaseArmor()
    {
        return characterData != null ? characterData.armor : 0;
    }

    private float GetBaseMoveSpeed()
    {
        return characterData != null ? characterData.moveSpeed : 0f;
    }

    private float GetBaseAttackSpeed()
    {
        return characterData != null ? characterData.attackSpeed : 0f;
    }

    private float GetBaseJumpForce()
    {
        return characterData != null ? characterData.jumpForce : 0f;
    }

    private int GetBonusHP(PlayerBonus bonusStats = null)
    {
        bonusStats ??= ResolveBonusStats();
        return bonusStats != null ? bonusStats.bonusHP : 0;
    }

    private int GetBonusDamage(PlayerBonus bonusStats = null)
    {
        bonusStats ??= ResolveBonusStats();
        return bonusStats != null ? bonusStats.bonusDamage : 0;
    }

    private int GetBonusArmor(PlayerBonus bonusStats = null)
    {
        bonusStats ??= ResolveBonusStats();
        return bonusStats != null ? bonusStats.bonusArmor : 0;
    }

    private float GetBonusMoveSpeed(PlayerBonus bonusStats = null)
    {
        bonusStats ??= ResolveBonusStats();
        return bonusStats != null ? bonusStats.bonusMoveSpeed : 0f;
    }

    private float GetBonusAttackSpeed(PlayerBonus bonusStats = null)
    {
        bonusStats ??= ResolveBonusStats();
        return bonusStats != null ? bonusStats.bonusAttackSpeed : 0f;
    }

    private float GetBonusJumpForce(PlayerBonus bonusStats = null)
    {
        bonusStats ??= ResolveBonusStats();
        return bonusStats != null ? bonusStats.bonusJumpForce : 0f;
    }

    private int GetBonusJumpCount(PlayerBonus bonusStats = null)
    {
        bonusStats ??= ResolveBonusStats();
        return bonusStats != null ? bonusStats.bonusJumpCount : 0;
    }
}