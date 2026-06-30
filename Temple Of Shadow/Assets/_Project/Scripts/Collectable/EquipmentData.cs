using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Equipment")]
public class EquipmentData : ItemData
{
    [Header("Stat Bonuses")]
    public int maxHP;
    public int damage;
    public int armor;

    public float moveSpeed;
    public float attackSpeed;
    public float jumpForce;
}