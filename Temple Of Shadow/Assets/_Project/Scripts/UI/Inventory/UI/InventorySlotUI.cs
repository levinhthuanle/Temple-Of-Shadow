using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI amountText;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image amountBadge;
    [SerializeField] private Image accentBar;

    private ItemData currentItem;
    private int currentAmount;
    private InventoryUI inventoryUI;
    private Button button;
    private bool isHovering;

    private void Awake()
    {
        EnsureReferences();
        ApplyStyle();

        button = GetComponent<Button>();
        if (button != null)
        {
            button.targetGraphic = backgroundImage;
            button.onClick.AddListener(OnClick);
            ConfigureButtonColors();
        }

        SetItem(null, 0);
    }

    private void Reset()
    {
        EnsureReferences();
        ApplyStyle();
    }

    public void Initialize(ItemData item, int amount, InventoryUI ui)
    {
        currentItem = item;
        currentAmount = amount;
        inventoryUI = ui;

        SetItem(item, amount);
    }

    public void SetItem(ItemData item, int amount)
    {
        EnsureReferences();

        currentItem = item;
        currentAmount = amount;

        if (item == null)
        {
            icon.enabled = false;
            amountText.text = string.Empty;
            amountBadge.enabled = false;
            accentBar.enabled = false;
            ApplyStyle();
            return;
        }

        icon.enabled = item.icon != null;
        icon.sprite = item.icon;
        amountText.text = amount > 1 ? amount.ToString() : string.Empty;
        amountBadge.enabled = amount > 1;
        accentBar.enabled = true;
        ApplyStyle();
    }

    public void EnsureReferences()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
            if (backgroundImage == null)
            {
                backgroundImage = gameObject.AddComponent<Image>();
            }
        }

        if (icon == null)
        {
            Transform iconTransform = transform.Find("ItemIcon");
            icon = iconTransform != null
                ? iconTransform.GetComponent<Image>()
                : CreateIcon();
        }

        if (amountBadge == null)
        {
            Transform badgeTransform = transform.Find("AmountBadge");
            amountBadge = badgeTransform != null
                ? badgeTransform.GetComponent<Image>()
                : CreateAmountBadge();
        }

        if (accentBar == null)
        {
            Transform accentTransform = transform.Find("AccentBar");
            accentBar = accentTransform != null
                ? accentTransform.GetComponent<Image>()
                : CreateAccentBar();
        }

        if (amountText == null)
        {
            Transform amountTransform = transform.Find("AmountText");
            amountText = amountTransform != null
                ? amountTransform.GetComponent<TextMeshProUGUI>()
                : CreateAmountText();
        }

        if (amountBadge != null)
        {
            amountBadge.transform.SetAsLastSibling();
        }

        if (accentBar != null)
        {
            accentBar.transform.SetAsLastSibling();
        }

        if (amountText != null)
        {
            amountText.transform.SetAsLastSibling();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            return;
        }

        if (currentItem == null || inventoryUI == null)
        {
            return;
        }

        inventoryUI.OnItemRightClicked(currentItem);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        ApplyStyle();

        if (currentItem == null || inventoryUI == null)
        {
            return;
        }

        inventoryUI.OnItemHovered(currentItem, currentAmount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        ApplyStyle();

        if (inventoryUI == null)
        {
            return;
        }

        inventoryUI.OnItemHoverExit(currentItem);
    }

    private void OnClick()
    {
        if (currentItem == null || inventoryUI == null)
        {
            return;
        }

        inventoryUI.OnItemClicked(currentItem);
    }

    private void ApplyStyle()
    {
        LayoutElement layoutElement = InventoryUITheme.EnsureLayoutElement(gameObject);
        layoutElement.minWidth = 88f;
        layoutElement.minHeight = 88f;
        layoutElement.preferredWidth = 92f;
        layoutElement.preferredHeight = 92f;

        transform.localScale = isHovering && currentItem != null
            ? new Vector3(1.02f, 1.02f, 1f)
            : Vector3.one;

        if (backgroundImage != null)
        {
            Color slotColor = GetSlotColor();
            backgroundImage.color = isHovering && currentItem != null
                ? Color.Lerp(slotColor, InventoryUITheme.SlotHover, 0.32f)
                : slotColor;
            backgroundImage.raycastTarget = true;
        }

        InventoryUITheme.EnsureOutline(
            gameObject,
            isHovering && currentItem != null ? GetAccentColor() : InventoryUITheme.BorderSoft,
            isHovering && currentItem != null ? new Vector2(3f, -3f) : new Vector2(1.5f, -1.5f));

        if (icon != null)
        {
            icon.color = currentItem == null
                ? new Color(1f, 1f, 1f, 0f)
                : Color.white;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
        }

        if (amountText != null)
        {
            amountText.color = InventoryUITheme.TextPrimary;
            amountText.fontSize = 18f;
            amountText.fontStyle = FontStyles.Bold;
            amountText.alignment = TextAlignmentOptions.BottomRight;
            amountText.raycastTarget = false;
        }

        if (amountBadge != null)
        {
            amountBadge.color = new Color(0.035f, 0.038f, 0.046f, 0.92f);
            amountBadge.raycastTarget = false;
        }

        if (accentBar != null)
        {
            accentBar.enabled = currentItem != null;
            accentBar.color = GetAccentColor();
            accentBar.raycastTarget = false;
        }
    }

    private void ConfigureButtonColors()
    {
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.pressedColor = new Color(0.85f, 0.72f, 0.45f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.45f);
        colors.colorMultiplier = 1f;
        button.colors = colors;
    }

    private Color GetSlotColor()
    {
        if (currentItem == null)
        {
            return InventoryUITheme.SlotEmpty;
        }

        if (currentItem is EquipmentData)
        {
            return InventoryUITheme.SlotEquipment;
        }

        if (currentItem.itemType == ItemType.Potion)
        {
            return InventoryUITheme.SlotConsumable;
        }

        return InventoryUITheme.SlotFilled;
    }

    private Color GetAccentColor()
    {
        if (currentItem != null && currentItem.itemType == ItemType.Potion)
        {
            return InventoryUITheme.ConsumableAccent;
        }

        if (currentItem is EquipmentData)
        {
            return InventoryUITheme.EquipmentAccent;
        }

        return InventoryUITheme.Accent;
    }

    private Image CreateIcon()
    {
        GameObject iconObject = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(12f, 14f);
        rectTransform.offsetMax = new Vector2(-12f, -14f);

        Image image = iconObject.GetComponent<Image>();
        image.raycastTarget = false;
        image.preserveAspect = true;
        return image;
    }

    private Image CreateAmountBadge()
    {
        GameObject badgeObject = new GameObject("AmountBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        badgeObject.transform.SetParent(transform, false);

        RectTransform rectTransform = badgeObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(1f, 0f);
        rectTransform.anchoredPosition = new Vector2(-6f, 6f);
        rectTransform.sizeDelta = new Vector2(30f, 22f);

        Image image = badgeObject.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private Image CreateAccentBar()
    {
        GameObject accentObject = new GameObject("AccentBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accentObject.transform.SetParent(transform, false);

        RectTransform rectTransform = accentObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.sizeDelta = new Vector2(-18f, 5f);

        Image image = accentObject.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private TextMeshProUGUI CreateAmountText()
    {
        GameObject textObject = new GameObject("AmountText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(4f, 2f);
        rectTransform.offsetMax = new Vector2(-7f, -4f);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.BottomRight;
        text.fontSize = 18f;
        text.fontStyle = FontStyles.Bold;
        text.raycastTarget = false;
        return text;
    }
}
