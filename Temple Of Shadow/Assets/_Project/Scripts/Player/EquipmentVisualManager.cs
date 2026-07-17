using System;
using Spriter2UnityDX;
using UnityEngine;

public class EquipmentVisualManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer swordRenderer;
    [SerializeField] private SpriteRenderer armorRenderer;

    [Header("Defaults")]
    [SerializeField] private Sprite defaultSwordSprite;
    [SerializeField] private Sprite defaultArmorSprite;
    [SerializeField] private bool hideSwordWhenEmpty = true;
    [SerializeField] private bool hideArmorWhenEmpty = true;

    [Header("Armor Sorting")]
    [SerializeField] private bool syncArmorSortingWithBody = true;
    [SerializeField] private int armorSortingOrderOffset = 1;

    private Sprite currentSwordSprite;
    private Sprite currentArmorSprite;

    private void Awake()
    {
        ResolveReferences();
        ConfigureArmorSorting();
    }

    private void OnEnable()
    {
        ResolveReferences();

        if (equipmentManager != null)
        {
            equipmentManager.EquipmentChanged += RefreshVisuals;
        }

        RefreshVisuals();
    }

    private void OnDisable()
    {
        if (equipmentManager != null)
        {
            equipmentManager.EquipmentChanged -= RefreshVisuals;
        }
    }

    public void RefreshVisuals()
    {
        ResolveReferences();
        ConfigureArmorSorting();
        ApplySwordVisual();
        ApplyArmorVisual();
    }

    private void LateUpdate()
    {
        // Spriter animations can write SpriteRenderer.sprite after gameplay Update.
        ReapplySpriteIfAnimationOverrodeIt(swordRenderer, currentSwordSprite);
        ReapplySpriteIfAnimationOverrodeIt(armorRenderer, currentArmorSprite);
    }

    private void ApplySwordVisual()
    {
        if (swordRenderer == null)
        {
            return;
        }

        EquipmentData equippedSword = equipmentManager != null
            ? equipmentManager.GetEquippedEquipment(EquipmentSlotType.Sword)
            : null;

        Sprite sprite = equippedSword != null && equippedSword.VisualSprite != null
            ? equippedSword.VisualSprite
            : equippedSword != null && equippedSword.icon != null
                ? equippedSword.icon
                : defaultSwordSprite;

        currentSwordSprite = sprite;
        ApplySprite(swordRenderer, currentSwordSprite, hideSwordWhenEmpty);
    }

    private void ApplyArmorVisual()
    {
        if (armorRenderer == null)
        {
            return;
        }

        EquipmentData equippedArmor = equipmentManager != null
            ? equipmentManager.GetEquippedEquipment(EquipmentSlotType.Armor)
            : null;

        Sprite sprite = equippedArmor != null && equippedArmor.VisualSprite != null
            ? equippedArmor.VisualSprite
            : equippedArmor != null && equippedArmor.icon != null
                ? equippedArmor.icon
                : defaultArmorSprite;

        currentArmorSprite = sprite;
        ApplySprite(armorRenderer, currentArmorSprite, hideArmorWhenEmpty);
    }

    private void ConfigureArmorSorting()
    {
        if (!syncArmorSortingWithBody || bodyRenderer == null || armorRenderer == null)
        {
            return;
        }

        armorRenderer.sortingLayerID = bodyRenderer.sortingLayerID;
        armorRenderer.sortingOrder = bodyRenderer.sortingOrder + armorSortingOrderOffset;
    }

    private void ApplySprite(SpriteRenderer targetRenderer, Sprite sprite, bool hideWhenEmpty)
    {
        if (targetRenderer == null)
        {
            return;
        }

        targetRenderer.sprite = sprite;
        targetRenderer.enabled = sprite != null || !hideWhenEmpty;

        TextureController textureController = targetRenderer.GetComponent<TextureController>();
        if (textureController == null || sprite == null)
        {
            return;
        }

        if (textureController.Sprites == null || textureController.Sprites.Length == 0)
        {
            textureController.Sprites = new[] { sprite };
        }
        else
        {
            for (int i = 0; i < textureController.Sprites.Length; i++)
            {
                textureController.Sprites[i] = sprite;
            }
        }
    }

    private void ReapplySpriteIfAnimationOverrodeIt(SpriteRenderer targetRenderer, Sprite expectedSprite)
    {
        if (targetRenderer == null || expectedSprite == null)
        {
            return;
        }

        if (targetRenderer.sprite != expectedSprite)
        {
            targetRenderer.sprite = expectedSprite;
        }
    }

    private void ResolveReferences()
    {
        if (equipmentManager == null)
        {
            equipmentManager = FindAnyObjectByType<EquipmentManager>();
        }

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        bodyRenderer ??= FindRendererByName(renderers, "Body");
        swordRenderer ??= FindRendererByName(renderers, "Sword");
        armorRenderer ??= FindRendererByName(renderers, "Armor");
    }

    private SpriteRenderer FindRendererByName(SpriteRenderer[] renderers, string namePart)
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return renderer;
            }
        }

        return null;
    }
}
