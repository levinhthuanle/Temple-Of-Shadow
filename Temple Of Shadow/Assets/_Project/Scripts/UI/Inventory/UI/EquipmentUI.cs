using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public EquipmentManager equipmentManager;

    public Image swordIcon;
    public Image armorIcon;
    public Image accessoryIcon;
    public Image projectileIcon;

    private void Awake()
    {
        ResolveReferences();
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
            equipmentManager.equippedSword);

        UpdateSlot(
            armorIcon,
            equipmentManager.equippedArmor);

        UpdateSlot(
            accessoryIcon,
            equipmentManager.equippedAccessory);

        UpdateSlot(
            projectileIcon,
            equipmentManager.equippedProjectile);
    }

    private void ResolveReferences()
    {
        if (equipmentManager == null)
        {
            equipmentManager = FindAnyObjectByType<EquipmentManager>();
        }
    }

    private void UpdateSlot(
        Image image,
        EquipmentData equipment)
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
    }
}
