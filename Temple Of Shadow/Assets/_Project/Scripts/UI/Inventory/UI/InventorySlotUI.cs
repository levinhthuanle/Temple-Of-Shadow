using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TextMeshProUGUI amountText;
    private ItemData currentItem;
    private InventoryUI inventoryUI;
    private Button button;

    private void Awake()
    {
        EnsureReferences();
        SetItem(null, 0);

        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }

    }

    private void OnClick()
    {
        if (currentItem == null || inventoryUI == null)
            return;

        inventoryUI.OnItemClicked(currentItem);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null || inventoryUI == null)
        {
            return;
        }

        inventoryUI.OnItemHovered(currentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventoryUI == null)
        {
            return;
        }

        inventoryUI.OnItemHoverExit(currentItem);
    }

    private void Reset()
    {
        EnsureReferences();
    }

    public void Initialize(
    ItemData item,
    int amount,
    InventoryUI ui)
    {
        currentItem = item;
        inventoryUI = ui;

        SetItem(item, amount);
    }


    public void EnsureReferences()
    {
        if (icon == null)
        {
            Transform iconTransform = transform.Find("ItemIcon");
            icon = iconTransform != null
                ? iconTransform.GetComponent<Image>()
                : CreateIcon();
        }

        if (amountText == null)
        {
            Transform amountTransform = transform.Find("AmountText");
            amountText = amountTransform != null
                ? amountTransform.GetComponent<TextMeshProUGUI>()
                : CreateAmountText();
        }
    }

    public void SetItem(ItemData item, int amount)
    {
        EnsureReferences();
        currentItem = item;

        if (item == null)
        {
            icon.enabled = false;
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = item.icon;

        amountText.text = amount > 1
            ? amount.ToString()
            : "";
    }

    private Image CreateIcon()
    {
        GameObject iconObject = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(10f, 10f);
        rectTransform.offsetMax = new Vector2(-10f, -10f);

        Image image = iconObject.GetComponent<Image>();
        image.raycastTarget = false;
        image.preserveAspect = true;
        return image;
    }

    private TextMeshProUGUI CreateAmountText()
    {
        GameObject textObject = new GameObject("AmountText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.offsetMin = new Vector2(4f, 2f);
        rectTransform.offsetMax = new Vector2(-4f, -2f);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.BottomRight;
        text.fontSize = 22f;
        text.raycastTarget = false;
        return text;
    }
}
