using System;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public event Action EquipmentChanged;

    public EquipmentData equippedSword;
    public EquipmentData equippedArmor;
    public EquipmentData equippedAccessory;
    public EquipmentData equippedProjectile;

    public PlayerBonus playerBonus;

    private EquipmentData cachedSword;
    private EquipmentData cachedArmor;
    private EquipmentData cachedAccessory;
    private EquipmentData cachedProjectile;

    public EquipmentUI equipmentUI;
    private EquipmentVisualManager equipmentVisualManager;

    [SerializeField] private EquipmentData testSword;

    private void Start()
    {
        ResolveEquipmentUI();
        ResolvePlayerBonus();
        ResolveEquipmentVisualManager();
        RecalculateBonuses();
        RefreshEquipmentUI();
        EquipmentChanged?.Invoke();
    }

    private void Update()
    {
        if (!HasEquipmentChanged())
        {
            return;
        }

        HandleEquipmentStateChanged();
    }

    public EquipmentData Equip(EquipmentData equipment)
    {
        if (equipment == null)
        {
            return null;
        }

        if (!CanEquip(equipment.SlotType))
        {
            Debug.LogWarning($"Cannot equip item slot {equipment.SlotType} as equipment.");
            return null;
        }

        EquipmentData previousEquipment = GetEquippedEquipment(equipment.SlotType);

        if (previousEquipment == equipment)
        {
            return previousEquipment;
        }

        switch (equipment.SlotType)
        {
            case EquipmentSlotType.Sword:
                equippedSword = equipment;
                break;
            case EquipmentSlotType.Armor:
                equippedArmor = equipment;
                break;
            case EquipmentSlotType.Accessory:
                equippedAccessory = equipment;
                break;
            case EquipmentSlotType.Projectile:
                equippedProjectile = equipment;
                break;
        }

        HandleEquipmentStateChanged();
        return previousEquipment;
    }

    public void EquipSword(EquipmentData equipment)
    {
        equippedSword = equipment;
        HandleEquipmentStateChanged();
    }

    public void EquipArmor(EquipmentData equipment)
    {
        equippedArmor = equipment;
        HandleEquipmentStateChanged();
    }

    public void EquipAccessory(EquipmentData equipment)
    {
        equippedAccessory = equipment;
        HandleEquipmentStateChanged();
    }

    public void EquipProjectile(EquipmentData equipment)
    {
        equippedProjectile = equipment;
        HandleEquipmentStateChanged();
    }

    public void ApplyLoadout(
        EquipmentData sword,
        EquipmentData armor,
        EquipmentData accessory,
        EquipmentData projectile)
    {
        equippedSword = sword;
        equippedArmor = armor;
        equippedAccessory = accessory;
        equippedProjectile = projectile;
        HandleEquipmentStateChanged();
    }

    public void RecalculateBonuses()
    {
        ResolvePlayerBonus();

        if (playerBonus == null)
        {
            Debug.LogWarning("[EquipmentManager] Missing PlayerBonus. Assign the player bonus target in the Inspector.");
            return;
        }

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

    private void ResolvePlayerBonus()
    {
        if (playerBonus == null)
        {
            playerBonus = FindAnyObjectByType<PlayerBonus>();
        }
    }

    private void ResolveEquipmentUI()
    {
        if (equipmentUI == null)
        {
            equipmentUI = FindAnyObjectByType<EquipmentUI>();
        }
    }

    private void ResolveEquipmentVisualManager()
    {
        if (equipmentVisualManager != null)
        {
            return;
        }

        equipmentVisualManager = FindAnyObjectByType<EquipmentVisualManager>();
        if (equipmentVisualManager != null)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null && playerBonus != null)
        {
            player = playerBonus.gameObject;
        }

        if (player != null)
        {
            equipmentVisualManager = player.GetComponent<EquipmentVisualManager>();
            if (equipmentVisualManager == null)
            {
                equipmentVisualManager = player.AddComponent<EquipmentVisualManager>();
            }
        }
    }

    private void RefreshEquipmentUI()
    {
        ResolveEquipmentUI();

        if (equipmentUI == null)
        {
            Debug.LogWarning("[EquipmentManager] Missing EquipmentUI. Add EquipmentUI to the scene or assign it in the Inspector.");
            return;
        }

        equipmentUI.Refresh();
    }

    private void HandleEquipmentStateChanged()
    {
        RecalculateBonuses();
        RefreshEquipmentUI();
        ResolveEquipmentVisualManager();

        if (equipmentVisualManager != null)
        {
            equipmentVisualManager.RefreshVisuals();
        }

        EquipmentChanged?.Invoke();
    }

    public bool CanEquip(ItemType itemType)
    {
        return CanEquip(EquipmentSlotTypeExtensions.FromItemType(itemType));
    }

    public bool CanEquip(EquipmentSlotType slotType)
    {
        switch (slotType)
        {
            case EquipmentSlotType.Sword:
            case EquipmentSlotType.Armor:
            case EquipmentSlotType.Accessory:
            case EquipmentSlotType.Projectile:
                return true;
            default:
                return false;
        }
    }

    public EquipmentData GetEquippedEquipment(ItemType itemType)
    {
        return GetEquippedEquipment(EquipmentSlotTypeExtensions.FromItemType(itemType));
    }

    public EquipmentData GetEquippedEquipment(EquipmentSlotType slotType)
    {
        switch (slotType)
        {
            case EquipmentSlotType.Sword:
                return equippedSword;
            case EquipmentSlotType.Armor:
                return equippedArmor;
            case EquipmentSlotType.Accessory:
                return equippedAccessory;
            case EquipmentSlotType.Projectile:
                return equippedProjectile;
            default:
                return null;
        }
    }

    public void Unequip(ItemType itemType)
    {
        Unequip(EquipmentSlotTypeExtensions.FromItemType(itemType));
    }

    public void Unequip(EquipmentSlotType slotType)
    {
        switch (slotType)
        {
            case EquipmentSlotType.Sword:
                equippedSword = null;
                break;

            case EquipmentSlotType.Armor:
                equippedArmor = null;
                break;

            case EquipmentSlotType.Accessory:
                equippedAccessory = null;
                break;

            case EquipmentSlotType.Projectile:
                equippedProjectile = null;
                break;
        }

        HandleEquipmentStateChanged();
    }
}
