using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Equipment")]
public class EquipmentData : ItemData
{
    [Header("Equipment Slot")]
    public EquipmentSlotType equipmentSlotType = EquipmentSlotType.None;

    [Header("Equipment Visual")]
    public Sprite visualSprite;

    [Header("Stat Bonuses")]
    public int maxHP;
    public int damage;
    public int armor;

    public float moveSpeed;
    public float attackSpeed;
    public float jumpForce;

    public EquipmentSlotType SlotType
    {
        get
        {
            if (equipmentSlotType != EquipmentSlotType.None)
            {
                return equipmentSlotType;
            }

            return EquipmentSlotTypeExtensions.FromItemType(itemType);
        }
    }

    public Sprite VisualSprite => visualSprite;

    private void OnEnable()
    {
        SyncSerializedState();
    }

    private void OnValidate()
    {
        SyncSerializedState();
    }

    private void SyncSerializedState()
    {
        if (equipmentSlotType == EquipmentSlotType.None)
        {
            equipmentSlotType = EquipmentSlotTypeExtensions.FromItemType(itemType);
        }

        if (equipmentSlotType != EquipmentSlotType.None)
        {
            itemType = equipmentSlotType.ToItemType();
        }

        stackable = false;
    }
}
