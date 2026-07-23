using UnityEngine;

public abstract class ConsumableItemData : ItemData
{
    [Header("Consumable")]
    [SerializeField] private bool consumeOnlyWhenEffectApplies = true;

    public bool ConsumeOnlyWhenEffectApplies => consumeOnlyWhenEffectApplies;

    public abstract bool CanUse(GameObject user);

    public abstract bool Use(GameObject user);

    protected virtual void OnEnable()
    {
        ApplyConsumableDefaults();
    }

    protected virtual void OnValidate()
    {
        ApplyConsumableDefaults();
    }

    private void ApplyConsumableDefaults()
    {
        itemType = ItemType.Potion;
        stackable = true;
    }
}
