using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopDatabase database;
    [SerializeField] private ShopItemUI shopItemPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlayerWallet playerWallet;

    private readonly List<ShopItemUI> spawnedItems = new();
    private readonly List<ItemData> activeItems = new();
    private GameObject shopPanel;
    private TextMeshProUGUI shopTitle;

    private void OnEnable()
    {
        ResolveReferences();
        Subscribe();
        EnsureDefaultUI();
        CloseShop();
    }

    private void OnDisable() => Unsubscribe();

    public void Refresh()
    {
        ResolveReferences();
        EnsureDefaultUI();
        ClearSpawnedItems();

        if (database == null || shopItemPrefab == null || content == null)
        {
            Debug.LogWarning("[ShopUI] Assign ShopDatabase, Shop Item Prefab and ScrollView Content in the Inspector.", this);
            return;
        }

        IReadOnlyList<ItemData> items = activeItems.Count > 0 ? activeItems : database.itemsForSale;
        foreach (ItemData item in items)
        {
            if (item == null)
                continue;

            ShopItemUI itemUI = Instantiate(shopItemPrefab, content);
            itemUI.gameObject.SetActive(true);
            itemUI.Initialize(item, this);
            spawnedItems.Add(itemUI);
        }
    }

    public void OpenShop(string title, IEnumerable<ItemData> items, bool alignRight)
    {
        EnsureDefaultUI();
        activeItems.Clear();
        if (items != null)
            activeItems.AddRange(items);

        if (shopTitle != null)
            shopTitle.text = title;

        RectTransform panelRect = shopPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = alignRight ? new Vector2(0.56f, 0.18f) : new Vector2(0.04f, 0.18f);
        panelRect.anchorMax = alignRight ? new Vector2(0.96f, 0.9f) : new Vector2(0.44f, 0.9f);
        shopPanel.SetActive(true);
        Refresh();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    public bool CanBuy(ItemData item)
    {
        return item != null && item.buyPrice >= 0
            && playerWallet != null && inventoryManager != null
            && playerWallet.CanAfford(item.buyPrice)
            && inventoryManager.CanAddItem(item);
    }

    public bool TryBuy(ItemData item)
    {
        if (!CanBuy(item))
        {
            Debug.LogWarning($"[ShopUI] Cannot buy {(item != null ? item.itemName : "null")}: check gold and inventory space.", this);
            RefreshButtons();
            return false;
        }

        if (!playerWallet.TrySpendGold(item.buyPrice))
            return false;

        if (!inventoryManager.AddItem(item))
        {
            playerWallet.AddGold(item.buyPrice);
            Debug.LogError($"[ShopUI] Rolled back purchase of {item.itemName}; inventory rejected it.", this);
            return false;
        }

        Debug.Log($"[ShopUI] Bought {item.itemName} for {item.buyPrice} Gold.", this);
        RefreshButtons();
        return true;
    }

    private void ResolveReferences()
    {
        database ??= FindAnyObjectByType<ShopDatabase>();
        inventoryManager ??= FindAnyObjectByType<InventoryManager>();
        playerWallet ??= FindAnyObjectByType<PlayerWallet>();

        if (content != null)
            return;

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "Content")
            {
                content = child;
                break;
            }
        }
    }

    private void EnsureDefaultUI()
    {
        if (content != null && shopItemPrefab != null)
            return;

        GameObject panel = CreateUIObject("ShopPanel", transform);
        shopPanel = panel;
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.035f, 0.027f, 0.055f, 0.97f);
        SetRect(panel.GetComponent<RectTransform>(), new Vector2(0.04f, 0.18f), new Vector2(0.44f, 0.9f), Vector2.zero, Vector2.zero);

        Outline panelOutline = panel.AddComponent<Outline>();
        panelOutline.effectColor = new Color(0.72f, 0.48f, 0.12f, 1f);
        panelOutline.effectDistance = new Vector2(4f, -4f);

        TextMeshProUGUI title = CreateText("Title", panel.transform, "WEAPON FORGE", 34, TextAlignmentOptions.Center);
        shopTitle = title;
        title.color = new Color(1f, 0.78f, 0.28f);
        SetRect(title.rectTransform, new Vector2(0.05f, 0.86f), new Vector2(0.95f, 0.98f), Vector2.zero, Vector2.zero);

        GameObject viewport = CreateUIObject("Viewport", panel.transform);
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0.09f, 0.075f, 0.12f, 0.9f);
        viewport.AddComponent<RectMask2D>();
        SetRect(viewport.GetComponent<RectTransform>(), new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.84f), Vector2.zero, Vector2.zero);

        GameObject contentObject = CreateUIObject("Content", viewport.transform);
        content = contentObject.transform;
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.sizeDelta = Vector2.zero;
        GridLayoutGroup layout = contentObject.AddComponent<GridLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = new Vector2(10f, 10f);
        layout.cellSize = new Vector2(285f, 86f);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 2;
        ContentSizeFitter fitter = contentObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = panel.AddComponent<ScrollRect>();
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = contentRect;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        GameObject template = CreateUIObject("ShopItemSlot", panel.transform);
        template.SetActive(false);
        LayoutElement element = template.AddComponent<LayoutElement>();
        element.preferredHeight = 86f;
        Image background = template.AddComponent<Image>();
        background.color = new Color(0.12f, 0.095f, 0.17f, 1f);
        Outline cardOutline = template.AddComponent<Outline>();
        cardOutline.effectColor = new Color(0.38f, 0.25f, 0.08f, 1f);

        GameObject iconObject = CreateUIObject("Icon", template.transform);
        Image iconImage = iconObject.AddComponent<Image>();
        iconImage.preserveAspect = true;
        SetRect(iconObject.GetComponent<RectTransform>(), new Vector2(0.03f, 0.18f), new Vector2(0.24f, 0.82f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI nameText = CreateText("Name", template.transform, "Item", 18, TextAlignmentOptions.Left);
        nameText.enableAutoSizing = true;
        nameText.fontSizeMin = 13;
        SetRect(nameText.rectTransform, new Vector2(0.28f, 0.53f), new Vector2(0.72f, 0.9f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI price = CreateText("Price", template.transform, "0 Gold", 16, TextAlignmentOptions.Left);
        price.color = new Color(1f, 0.78f, 0.2f);
        SetRect(price.rectTransform, new Vector2(0.28f, 0.12f), new Vector2(0.72f, 0.52f), Vector2.zero, Vector2.zero);

        GameObject buttonObject = CreateUIObject("Buy Button", template.transform);
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.42f, 0.2f, 0.58f, 1f);
        buttonObject.AddComponent<Button>();
        SetRect(buttonObject.GetComponent<RectTransform>(), new Vector2(0.75f, 0.22f), new Vector2(0.97f, 0.78f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI buttonText = CreateText("Label", buttonObject.transform, "BUY", 16, TextAlignmentOptions.Center);
        SetRect(buttonText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        shopItemPrefab = template.AddComponent<ShopItemUI>();
    }

    private static GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject result = new GameObject(objectName, typeof(RectTransform));
        result.transform.SetParent(parent, false);
        return result;
    }

    private static TextMeshProUGUI CreateText(string objectName, Transform parent, string value, float size, TextAlignmentOptions alignment)
    {
        GameObject textObject = CreateUIObject(objectName, parent);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private void Subscribe()
    {
        Unsubscribe();
        if (playerWallet != null)
            playerWallet.GoldChanged += OnGoldChanged;
        if (inventoryManager != null)
            inventoryManager.InventoryChanged += RefreshButtons;
    }

    private void Unsubscribe()
    {
        if (playerWallet != null)
            playerWallet.GoldChanged -= OnGoldChanged;
        if (inventoryManager != null)
            inventoryManager.InventoryChanged -= RefreshButtons;
    }

    private void OnGoldChanged(int gold) => RefreshButtons();

    private void RefreshButtons()
    {
        foreach (ShopItemUI itemUI in spawnedItems)
        {
            if (itemUI != null)
                itemUI.RefreshButton();
        }
    }

    private void ClearSpawnedItems()
    {
        foreach (ShopItemUI itemUI in spawnedItems)
        {
            if (itemUI != null)
                Destroy(itemUI.gameObject);
        }
        spawnedItems.Clear();
    }
}
