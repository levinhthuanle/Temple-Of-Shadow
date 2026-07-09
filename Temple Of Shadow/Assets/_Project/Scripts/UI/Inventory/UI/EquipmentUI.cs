using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public EquipmentManager equipmentManager;

    public Image swordIcon;
    public Image armorIcon;
    public Image accessoryIcon;
    public Image projectileIcon;

    private EquipmentIconUI swordHoverUI;
    private EquipmentIconUI armorHoverUI;
    private EquipmentIconUI accessoryHoverUI;
    private EquipmentIconUI projectileHoverUI;

    private void Awake()
    {
        ResolveReferences();
        ResolveHoverComponents();
    }

    public void Refresh()
    {
        ResolveReferences();

        if (equipmentManager == null)
        {
            Debug.LogWarning("[EquipmentUI] Missing EquipmentManager. Add EquipmentManager to the scene or assign it in the Inspector.");
            return;
        }

        UpdateSlot(
            swordIcon,
            equipmentManager.equippedSword,
            swordHoverUI);

        UpdateSlot(
            armorIcon,
            equipmentManager.equippedArmor,
            armorHoverUI);

        UpdateSlot(
            accessoryIcon,
            equipmentManager.equippedAccessory,
            accessoryHoverUI);

        UpdateSlot(
            projectileIcon,
            equipmentManager.equippedProjectile,
            projectileHoverUI);
    }

    private void ResolveReferences()
    {
        if (equipmentManager == null)
        {
            equipmentManager = FindAnyObjectByType<EquipmentManager>();
        }
    }

    private void ResolveHoverComponents()
    {
        swordHoverUI = EnsureHoverComponent(swordIcon, swordHoverUI);
        armorHoverUI = EnsureHoverComponent(armorIcon, armorHoverUI);
        accessoryHoverUI = EnsureHoverComponent(accessoryIcon, accessoryHoverUI);
        projectileHoverUI = EnsureHoverComponent(projectileIcon, projectileHoverUI);
    }

    private void UpdateSlot(
        Image image,
        EquipmentData equipment,
        EquipmentIconUI hoverUI)
    {
        if (image == null)
        {
            return;
        }

        if (equipment == null)
        {
            image.enabled = false;
            return;
        }

        image.enabled = true;
        image.sprite = equipment.icon;

        if (hoverUI != null)
        {
            hoverUI.SetEquipment(equipment);
        }
    }

    private EquipmentIconUI EnsureHoverComponent(Image image, EquipmentIconUI existing)
    {
        if (image == null)
        {
            return null;
        }

        if (existing != null)
        {
            return existing;
        }

        EquipmentIconUI hoverUI = image.GetComponent<EquipmentIconUI>();
        if (hoverUI == null)
        {
            hoverUI = image.gameObject.AddComponent<EquipmentIconUI>();
        }

        return hoverUI;
    }
}
