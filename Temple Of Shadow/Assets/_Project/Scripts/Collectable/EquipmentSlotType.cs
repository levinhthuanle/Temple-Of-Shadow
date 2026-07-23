public enum EquipmentSlotType
{
    None = 0,
    Sword = 1,
    Armor = 2,
    Projectile = 3,
    Accessory = 4
}

public static class EquipmentSlotTypeExtensions
{
    public static ItemType ToItemType(this EquipmentSlotType slotType)
    {
        switch (slotType)
        {
            case EquipmentSlotType.Sword:
                return ItemType.Sword;
            case EquipmentSlotType.Armor:
                return ItemType.Armor;
            case EquipmentSlotType.Projectile:
                return ItemType.Projectile;
            case EquipmentSlotType.Accessory:
                return ItemType.Accessory;
            default:
                return ItemType.Potion;
        }
    }

    public static EquipmentSlotType FromItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Sword:
                return EquipmentSlotType.Sword;
            case ItemType.Armor:
                return EquipmentSlotType.Armor;
            case ItemType.Projectile:
                return EquipmentSlotType.Projectile;
            case ItemType.Accessory:
                return EquipmentSlotType.Accessory;
            default:
                return EquipmentSlotType.None;
        }
    }
}
