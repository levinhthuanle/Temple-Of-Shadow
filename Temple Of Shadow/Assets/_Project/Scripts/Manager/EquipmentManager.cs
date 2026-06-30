using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public EquipmentData equippedSword;
    public EquipmentData equippedArmor;
    public EquipmentData equippedAccessory;
    public EquipmentData equippedProjectile;

    public PlayerBonus playerBonus;

    private void Start()
    {
        RecalculateBonuses();
    }

    private void RecalculateBonuses()
    {
        if (playerBonus == null)
            return;

        playerBonus.bonusHP = 0;
        playerBonus.bonusDamage = 0;
        playerBonus.bonusArmor = 0;

        playerBonus.bonusMoveSpeed = 0;
        playerBonus.bonusAttackSpeed = 0;
        playerBonus.bonusJumpForce = 0;

        ApplyEquipment(equippedSword);
        ApplyEquipment(equippedArmor);
        ApplyEquipment(equippedAccessory);
        ApplyEquipment(equippedProjectile);

        Debug.Log("Bonus Damage: " + playerBonus.bonusDamage);
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
}