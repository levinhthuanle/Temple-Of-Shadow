using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Consumables/Health Potion")]
public class HealthPotionData : ConsumableItemData
{
    public override bool CanUse(GameObject user)
    {
        PlayerHealth playerHealth = ResolvePlayerHealth(user);

        if (playerHealth == null)
        {
            return false;
        }

        return !ConsumeOnlyWhenEffectApplies
            || playerHealth.GetCurrentHp() < playerHealth.GetMaxHp();
    }

    public override bool Use(GameObject user)
    {
        PlayerHealth playerHealth = ResolvePlayerHealth(user);

        if (playerHealth == null)
        {
            Debug.LogWarning($"[HealthPotionData] Cannot use {itemName}: missing PlayerHealth target.");
            return false;
        }

        if (!CanUse(user))
        {
            Debug.Log($"[HealthPotionData] {itemName} was not consumed because HP is already full.");
            return false;
        }

        playerHealth.Heal(healAmount);
        return true;
    }

    private PlayerHealth ResolvePlayerHealth(GameObject user)
    {
        if (user == null)
        {
            return Object.FindAnyObjectByType<PlayerHealth>();
        }

        PlayerHealth playerHealth = user.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            return playerHealth;
        }

        return user.GetComponentInParent<PlayerHealth>();
    }
}
