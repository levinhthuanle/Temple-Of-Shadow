using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipUI tooltipUI;

    private Image iconImage;
    private EquipmentData currentEquipment;
    private bool isHovering;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        ResolveTooltip();
        ApplyStyle();
    }

    public void SetEquipment(EquipmentData equipment)
    {
        currentEquipment = equipment;

        if (iconImage == null)
        {
            iconImage = GetComponent<Image>();
        }

        if (iconImage == null)
        {
            return;
        }

        if (equipment == null)
        {
            iconImage.enabled = false;
            ApplyStyle();

            if (tooltipUI != null)
            {
                tooltipUI.Hide();
            }

            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = equipment.icon;
        ApplyStyle();

        if (isHovering && tooltipUI != null)
        {
            tooltipUI.Show(equipment);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        ApplyStyle();

        if (currentEquipment == null)
        {
            return;
        }

        ResolveTooltip();

        if (tooltipUI != null)
        {
            tooltipUI.Show(currentEquipment);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        ApplyStyle();

        ResolveTooltip();

        if (tooltipUI != null)
        {
            tooltipUI.Hide();
        }
    }

    private void ResolveTooltip()
    {
        if (tooltipUI == null)
        {
            tooltipUI = FindAnyObjectByType<TooltipUI>(FindObjectsInactive.Include);
        }
    }

    private void ApplyStyle()
    {
        if (iconImage == null)
        {
            iconImage = GetComponent<Image>();
        }

        if (iconImage != null)
        {
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = true;
        }

        InventoryUITheme.EnsureOutline(
            gameObject,
            isHovering && currentEquipment != null ? InventoryUITheme.Accent : InventoryUITheme.BorderSoft,
            isHovering && currentEquipment != null ? new Vector2(2f, -2f) : new Vector2(1f, -1f));
    }
}
