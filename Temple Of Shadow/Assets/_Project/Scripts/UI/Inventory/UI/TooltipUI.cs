using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI effectText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI descriptionText;

    [SerializeField] private Vector2 screenOffset = new Vector2(18f, -18f);
    [SerializeField] private float preferredWidth = 340f;
    [SerializeField] private float iconSize = 54f;

    private RectTransform rectTransform;
    private RectTransform headerRect;
    private RectTransform headerTextRect;
    private Canvas rootCanvas;
    private CanvasGroup canvasGroup;
    private Camera uiCamera;
    private Image backgroundImage;
    private Image iconImage;
    private Image dividerImage;

    private void Awake()
    {
        ResolveReferences();
        ApplyStyle();
        Hide();
    }

    private void Reset()
    {
        ResolveReferences();
        ApplyStyle();
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        FollowMouse();
    }

    public void Show(ItemData item)
    {
        Show(item, 1);
    }

    public void Show(ItemData item, int amount)
    {
        if (item == null)
        {
            Hide();
            return;
        }

        ResolveReferences();
        ApplyStyle();
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        iconImage.sprite = item.icon;
        iconImage.enabled = item.icon != null;
        iconImage.gameObject.SetActive(item.icon != null);

        SetTextVisible(nameText, item.itemName);
        SetTextVisible(typeText, BuildTypeText(item, amount));
        SetTextVisible(effectText, BuildEffectText(item));
        SetTextVisible(statsText, BuildStatsText(item));
        SetTextVisible(descriptionText, item.description);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(headerTextRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(headerRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        FollowMouse();
    }

    public void Hide()
    {
        ResolveReferences();
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void ResolveReferences()
    {
        rectTransform ??= GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        canvasGroup ??= GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        backgroundImage ??= InventoryUITheme.EnsureImage(gameObject);
        uiCamera = rootCanvas != null ? rootCanvas.worldCamera : null;

        headerRect = FindOrCreateRect(transform, "Header");
        headerTextRect = FindOrCreateRect(headerRect, "HeaderText");
        iconImage = FindOrCreateImage(headerRect, "Icon");
        dividerImage = FindOrCreateImage(transform, "Divider");

        nameText = ResolveText(nameText, "NameText", headerTextRect);
        typeText = ResolveText(typeText, "TypeText", headerTextRect);
        effectText = ResolveText(effectText, "EffectText", transform);
        statsText = ResolveText(statsText, "StatsText", transform);
        descriptionText = ResolveText(descriptionText, "DescriptionText", transform);

        headerRect.SetSiblingIndex(0);
        dividerImage.transform.SetSiblingIndex(1);
        effectText.transform.SetSiblingIndex(2);
        statsText.transform.SetSiblingIndex(3);
        descriptionText.transform.SetSiblingIndex(4);
    }

    private void ApplyStyle()
    {
        if (rectTransform != null)
        {
            rectTransform.pivot = new Vector2(0f, 1f);
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        backgroundImage.color = InventoryUITheme.PanelRaised;
        backgroundImage.raycastTarget = false;

        InventoryUITheme.EnsureOutline(gameObject, InventoryUITheme.Border, new Vector2(2f, -2f));
        InventoryUITheme.EnsureShadow(gameObject, new Color(0f, 0f, 0f, 0.58f), new Vector2(9f, -9f));

        LayoutElement layoutElement = InventoryUITheme.EnsureLayoutElement(gameObject);
        layoutElement.preferredWidth = preferredWidth;
        layoutElement.minWidth = 300f;
        layoutElement.flexibleWidth = 0f;

        VerticalLayoutGroup layoutGroup = GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(18, 18, 16, 16);
        layoutGroup.spacing = 8f;
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter fitter = GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = gameObject.AddComponent<ContentSizeFitter>();
        }

        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        StyleHeader();
        StyleText(nameText, 23f, InventoryUITheme.TextPrimary, FontStyles.Bold, TextWrappingModes.NoWrap);
        StyleText(typeText, 14f, InventoryUITheme.TextMuted, FontStyles.Normal, TextWrappingModes.NoWrap);
        StyleText(effectText, 15f, InventoryUITheme.ConsumableAccent, FontStyles.Bold, TextWrappingModes.Normal);
        StyleText(statsText, 15f, InventoryUITheme.TextPrimary, FontStyles.Normal, TextWrappingModes.Normal);
        StyleText(descriptionText, 14f, InventoryUITheme.TextMuted, FontStyles.Normal, TextWrappingModes.Normal);
    }

    private void StyleHeader()
    {
        HorizontalLayoutGroup headerLayout = headerRect.GetComponent<HorizontalLayoutGroup>();
        if (headerLayout == null)
        {
            headerLayout = headerRect.gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        headerLayout.spacing = 12f;
        headerLayout.childAlignment = TextAnchor.MiddleLeft;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = true;
        headerLayout.childForceExpandHeight = false;

        LayoutElement headerElement = InventoryUITheme.EnsureLayoutElement(headerRect.gameObject);
        headerElement.minHeight = 58f;
        headerElement.preferredHeight = 62f;

        LayoutElement iconElement = InventoryUITheme.EnsureLayoutElement(iconImage.gameObject);
        iconElement.minWidth = iconSize;
        iconElement.minHeight = iconSize;
        iconElement.preferredWidth = iconSize;
        iconElement.preferredHeight = iconSize;
        iconElement.flexibleWidth = 0f;

        iconImage.color = Color.white;
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;

        VerticalLayoutGroup textLayout = headerTextRect.GetComponent<VerticalLayoutGroup>();
        if (textLayout == null)
        {
            textLayout = headerTextRect.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        textLayout.spacing = 2f;
        textLayout.childAlignment = TextAnchor.MiddleLeft;
        textLayout.childControlWidth = true;
        textLayout.childControlHeight = true;
        textLayout.childForceExpandWidth = true;
        textLayout.childForceExpandHeight = false;

        LayoutElement textElement = InventoryUITheme.EnsureLayoutElement(headerTextRect.gameObject);
        textElement.minHeight = 54f;
        textElement.flexibleWidth = 1f;

        LayoutElement dividerElement = InventoryUITheme.EnsureLayoutElement(dividerImage.gameObject);
        dividerElement.minHeight = 1f;
        dividerElement.preferredHeight = 1f;
        dividerImage.color = InventoryUITheme.BorderSoft;
        dividerImage.raycastTarget = false;
    }

    private void FollowMouse()
    {
        if (rectTransform == null)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();

        float scaleFactor = rootCanvas != null ? Mathf.Max(rootCanvas.scaleFactor, 0.0001f) : 1f;
        Vector2 tooltipSize = rectTransform.rect.size * scaleFactor;
        Vector2 mousePosition = Input.mousePosition;
        Vector2 targetPosition = mousePosition + screenOffset;

        if (targetPosition.x + tooltipSize.x > Screen.width)
        {
            targetPosition.x = mousePosition.x - tooltipSize.x - Mathf.Abs(screenOffset.x);
        }

        if (targetPosition.y - tooltipSize.y < 0f)
        {
            targetPosition.y = mousePosition.y + tooltipSize.y + Mathf.Abs(screenOffset.y);
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, 8f, Mathf.Max(8f, Screen.width - tooltipSize.x - 8f));
        targetPosition.y = Mathf.Clamp(targetPosition.y, tooltipSize.y + 8f, Mathf.Max(tooltipSize.y + 8f, Screen.height - 8f));

        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransform canvasRect = rootCanvas.transform as RectTransform;
            if (canvasRect != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetPosition, uiCamera, out Vector2 localPoint))
            {
                rectTransform.anchoredPosition = localPoint;
                return;
            }
        }

        rectTransform.position = targetPosition;
    }

    private RectTransform FindOrCreateRect(Transform parent, string objectName)
    {
        Transform child = parent.Find(objectName);
        if (child != null && child.TryGetComponent(out RectTransform existingRect))
        {
            return existingRect;
        }

        GameObject rectObject = new GameObject(objectName, typeof(RectTransform));
        rectObject.transform.SetParent(parent, false);
        return rectObject.GetComponent<RectTransform>();
    }

    private Image FindOrCreateImage(Transform parent, string objectName)
    {
        Transform child = parent.Find(objectName);
        if (child != null && child.TryGetComponent(out Image existingImage))
        {
            return existingImage;
        }

        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private TextMeshProUGUI ResolveText(TextMeshProUGUI existingText, string objectName, Transform parent)
    {
        TextMeshProUGUI text = existingText;

        if (text == null)
        {
            Transform child = parent.Find(objectName);
            if (child != null)
            {
                text = child.GetComponent<TextMeshProUGUI>();
            }
        }

        if (text == null)
        {
            Transform legacyChild = transform.Find(objectName);
            if (legacyChild != null)
            {
                text = legacyChild.GetComponent<TextMeshProUGUI>();
            }
        }

        if (text == null)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            text = textObject.GetComponent<TextMeshProUGUI>();
        }
        else if (text.transform.parent != parent)
        {
            text.transform.SetParent(parent, false);
        }

        text.name = objectName;
        return text;
    }

    private void StyleText(TextMeshProUGUI text, float fontSize, Color color, FontStyles style, TextWrappingModes wrapping)
    {
        if (text == null)
        {
            return;
        }

        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Left;
        text.textWrappingMode = wrapping;
        text.overflowMode = wrapping == TextWrappingModes.NoWrap
            ? TextOverflowModes.Ellipsis
            : TextOverflowModes.Overflow;
        text.richText = true;
        text.raycastTarget = false;
        InventoryUITheme.EnsureShadow(text.gameObject, new Color(0f, 0f, 0f, 0.56f), new Vector2(1f, -1f));
    }

    private void SetTextVisible(TextMeshProUGUI text, string value)
    {
        if (text == null)
        {
            return;
        }

        bool hasText = !string.IsNullOrWhiteSpace(value);
        text.gameObject.SetActive(hasText);
        text.text = hasText ? value : string.Empty;
    }

    private string BuildTypeText(ItemData item, int amount)
    {
        string typeName = item.itemType.ToString();

        if (item is EquipmentData equipment)
        {
            typeName = $"{equipment.SlotType} Equipment";
        }
        else if (item.itemType == ItemType.Potion)
        {
            typeName = "Consumable";
        }

        if (amount > 1)
        {
            return $"{typeName}  x{amount}";
        }

        return typeName;
    }

    private string BuildEffectText(ItemData item)
    {
        if (item.itemType == ItemType.Potion)
        {
            return $"Restores {item.healAmount} HP";
        }

        return string.Empty;
    }

    private string BuildStatsText(ItemData item)
    {
        if (item is not EquipmentData equipment)
        {
            return string.Empty;
        }

        string stats = string.Empty;
        AppendStat(ref stats, "Max HP", equipment.maxHP);
        AppendStat(ref stats, "Damage", equipment.damage);
        AppendStat(ref stats, "Armor", equipment.armor);
        AppendStat(ref stats, "Move Speed", equipment.moveSpeed);
        AppendStat(ref stats, "Attack Speed", equipment.attackSpeed);
        AppendStat(ref stats, "Jump Force", equipment.jumpForce);

        return stats;
    }

    private void AppendStat(ref string stats, string label, int value)
    {
        if (value == 0)
        {
            return;
        }

        AppendLine(ref stats, $"{label}: <color=#{ColorUtility.ToHtmlStringRGB(InventoryUITheme.Positive)}>+{value}</color>");
    }

    private void AppendStat(ref string stats, string label, float value)
    {
        if (Mathf.Approximately(value, 0f))
        {
            return;
        }

        AppendLine(ref stats, $"{label}: <color=#{ColorUtility.ToHtmlStringRGB(InventoryUITheme.Positive)}>+{value:0.##}</color>");
    }

    private void AppendLine(ref string stats, string line)
    {
        if (!string.IsNullOrEmpty(stats))
        {
            stats += "\n";
        }

        stats += line;
    }
}
