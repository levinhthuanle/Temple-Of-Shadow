using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    private ItemData item;
    private ShopUI shopUI;

    private void Awake() => ResolveReferences();

    private void OnDestroy()
    {
        if (buyButton != null)
            buyButton.onClick.RemoveListener(Buy);
    }

    public void Initialize(ItemData itemData, ShopUI owner)
    {
        item = itemData;
        shopUI = owner;
        ResolveReferences();

        if (icon != null)
        {
            icon.sprite = item != null ? item.icon : null;
            icon.enabled = item != null && item.icon != null;
        }

        if (itemNameText != null)
            itemNameText.text = item != null ? item.itemName : "Unknown Item";
        if (priceText != null)
            priceText.text = item != null ? $"{item.buyPrice} Gold" : "--";

        RefreshButton();
    }

    public void RefreshButton()
    {
        if (buyButton != null)
            buyButton.interactable = item != null && shopUI != null && shopUI.CanBuy(item);
    }

    private void Buy()
    {
        if (item != null && shopUI != null)
            shopUI.TryBuy(item);
    }

    private void ResolveReferences()
    {
        icon ??= FindComponent<Image>("Icon");
        itemNameText ??= FindText("Name", "ItemName", "NameText");
        priceText ??= FindText("Price", "PriceText");
        buyButton ??= FindComponent<Button>("Buy Button") ?? FindComponent<Button>("BuyButton") ?? GetComponentInChildren<Button>(true);

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(Buy);
            buyButton.onClick.AddListener(Buy);
        }
    }

    private T FindComponent<T>(string childName) where T : Component
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
                return child.GetComponent<T>();
        }
        return null;
    }

    private TextMeshProUGUI FindText(params string[] names)
    {
        foreach (string childName in names)
        {
            TextMeshProUGUI text = FindComponent<TextMeshProUGUI>(childName);
            if (text != null)
                return text;
        }
        return null;
    }
}
