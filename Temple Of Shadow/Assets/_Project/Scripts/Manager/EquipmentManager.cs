using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public EquipmentData equippedSword;
    public EquipmentData equippedArmor;
    public EquipmentData equippedAccessory;
    public EquipmentData equippedProjectile;

    public PlayerBonus playerBonus;

    private EquipmentData cachedSword;
    private EquipmentData cachedArmor;
    private EquipmentData cachedAccessory;
    private EquipmentData cachedProjectile;

    private void Start()
    {
        RecalculateBonuses();
    }

    private void Update()
    {
        if (!HasEquipmentChanged())
        {
            return;
        }

        RecalculateBonuses();
    }

    public void Equip(EquipmentData equipment)
    {
        if (equipment == null)
        {
            return;
        }

        switch (equipment.itemType)
        {
            case ItemType.Sword:
                equippedSword = equipment;
                break;
            case ItemType.Armor:
                equippedArmor = equipment;
                break;
            case ItemType.Accessory:
                equippedAccessory = equipment;
                break;
            case ItemType.Projectile:
                equippedProjectile = equipment;
                break;
            default:
                Debug.LogWarning($"Cannot equip item type {equipment.itemType} as equipment.");
                return;
        }

        RecalculateBonuses();
    }

    public void EquipSword(EquipmentData equipment)
    {
        equippedSword = equipment;
        RecalculateBonuses();
    }

    public void EquipArmor(EquipmentData equipment)
    {
        equippedArmor = equipment;
        RecalculateBonuses();
    }

    public void EquipAccessory(EquipmentData equipment)
    {
        equippedAccessory = equipment;
        RecalculateBonuses();
    }

    public void EquipProjectile(EquipmentData equipment)
    {
        equippedProjectile = equipment;
        RecalculateBonuses();
    }

    public void RecalculateBonuses()
    {
        if (playerBonus == null)
            return;

        playerBonus.bonusHP = 0;
        playerBonus.bonusDamage = 0;
        playerBonus.bonusArmor = 0;

        playerBonus.bonusMoveSpeed = 0;
        playerBonus.bonusAttackSpeed = 0;
        playerBonus.bonusJumpForce = 0;
        playerBonus.bonusJumpCount = 0;

        ApplyEquipment(equippedSword);
        ApplyEquipment(equippedArmor);
        ApplyEquipment(equippedAccessory);
        ApplyEquipment(equippedProjectile);

        Debug.Log("Bonus Damage: " + playerBonus.bonusDamage);
        CacheEquipment();

        PlayerStats playerStats = playerBonus.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.RefreshStats();
        }
    }

    private void ApplyEquipment(EquipmentData equipment)
    {
        if (equipment == null)
            return;

        playerBonus.bonusHP += equipment.maxHP;
        playerBonus.bonusDamage += equipment.damage;
        playerBonus.bonusArmor += equipment.armor;

        playerBonus.bonusMoveSpeed += equipment.moveSpeed;
        playerBonus.bonusAttackSpeed += equipment.attackSpeed;
        playerBonus.bonusJumpForce += equipment.jumpForce;
    }

    private bool HasEquipmentChanged()
    {
        return cachedSword != equippedSword
            || cachedArmor != equippedArmor
            || cachedAccessory != equippedAccessory
            || cachedProjectile != equippedProjectile;
    }

    private void CacheEquipment()
    {
        cachedSword = equippedSword;
        cachedArmor = equippedArmor;
        cachedAccessory = equippedAccessory;
        cachedProjectile = equippedProjectile;
    }
}
