using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI jumpForceText;

    private PlayerStats subscribedStats;
    private PlayerHealth subscribedHealth;

    private void Awake()
    {
        ResolveReferences();
        ApplyStyle();
        Refresh();
    }

    private void OnEnable()
    {
        ResolveReferences();
        ApplyStyle();
        Subscribe();
        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Refresh()
    {
        ResolveReferences();

        if (playerStats == null)
        {
            Debug.LogWarning("[StatsPanelUI] Missing PlayerStats. Add PlayerStats to the player or assign it in the Inspector.");
            return;
        }

        int currentHp = playerHealth != null ? playerHealth.GetCurrentHp() : playerStats.MaxHP;
        int maxHp = playerHealth != null ? playerHealth.GetMaxHp() : playerStats.MaxHP;

        ApplyStyle();

        SetText(hpText, BuildStatLine("HP", $"{currentHp}/{maxHp}"));
        SetText(damageText, BuildStatLine("Damage", playerStats.Damage.ToString()));
        SetText(armorText, BuildStatLine("Armor", playerStats.Armor.ToString()));
        SetText(moveSpeedText, BuildStatLine("Move", FormatFloat(playerStats.MoveSpeed)));
        SetText(attackSpeedText, BuildStatLine("Attack", FormatFloat(playerStats.AttackSpeed)));
        SetText(jumpForceText, BuildStatLine("Jump", FormatFloat(playerStats.JumpForce)));
    }

    private void ResolveReferences()
    {
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (playerHealth == null)
        {
            playerHealth = playerStats != null
                ? playerStats.GetComponent<PlayerHealth>()
                : FindAnyObjectByType<PlayerHealth>();
        }

        hpText ??= FindText("HPText") ?? CreateText("HPText", 0);
        damageText ??= FindText("DamageText") ?? CreateText("DamageText", 1);
        armorText ??= FindText("ArmorText") ?? CreateText("ArmorText", 2);
        moveSpeedText ??= FindText("SpeedText") ?? CreateText("SpeedText", 3);
        attackSpeedText ??= FindText("AttackSpeedText") ?? CreateText("AttackSpeedText", 4);
        jumpForceText ??= FindText("JumpForceText") ?? CreateText("JumpForceText", 5);
    }

    private TextMeshProUGUI FindText(string objectName)
    {
        Transform child = transform.Find(objectName);
        return child != null ? child.GetComponent<TextMeshProUGUI>() : null;
    }

    private TextMeshProUGUI CreateText(string objectName, int siblingIndex)
    {
        TextMeshProUGUI template = hpText != null
            ? hpText
            : GetComponentInChildren<TextMeshProUGUI>(true);

        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(transform, false);
        textObject.transform.SetSiblingIndex(Mathf.Clamp(siblingIndex, 0, transform.childCount - 1));

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();

        if (template != null)
        {
            text.font = template.font;
            text.fontSize = template.fontSize;
            text.color = template.color;
            text.alignment = template.alignment;
            text.raycastTarget = template.raycastTarget;

            RectTransform templateRect = template.GetComponent<RectTransform>();
            rectTransform.sizeDelta = templateRect.sizeDelta;
            rectTransform.anchorMin = templateRect.anchorMin;
            rectTransform.anchorMax = templateRect.anchorMax;
            rectTransform.pivot = templateRect.pivot;
        }
        else
        {
            text.fontSize = 28f;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Left;
            rectTransform.sizeDelta = new Vector2(220f, 50f);
        }

        return text;
    }

    private void Subscribe()
    {
        if (playerStats != subscribedStats)
        {
            if (subscribedStats != null)
            {
                subscribedStats.StatsChanged -= Refresh;
            }

            subscribedStats = playerStats;

            if (subscribedStats != null)
            {
                subscribedStats.StatsChanged += Refresh;
            }
        }

        if (playerHealth != subscribedHealth)
        {
            if (subscribedHealth != null)
            {
                subscribedHealth.HealthChanged -= RefreshHealth;
            }

            subscribedHealth = playerHealth;

            if (subscribedHealth != null)
            {
                subscribedHealth.HealthChanged += RefreshHealth;
            }
        }
    }

    private void Unsubscribe()
    {
        if (subscribedStats != null)
        {
            subscribedStats.StatsChanged -= Refresh;
            subscribedStats = null;
        }

        if (subscribedHealth != null)
        {
            subscribedHealth.HealthChanged -= RefreshHealth;
            subscribedHealth = null;
        }
    }

    private void RefreshHealth(int currentHp, int maxHp)
    {
        SetText(hpText, BuildStatLine("HP", $"{currentHp}/{maxHp}"));
    }

    private void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }

    private string FormatFloat(float value)
    {
        return value.ToString("0.##");
    }

    private string BuildStatLine(string label, string value)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(InventoryUITheme.TextMuted)}>{label}</color>  <b>{value}</b>";
    }

    private void ApplyStyle()
    {
        VerticalLayoutGroup layoutGroup = GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            // Stat rows are positioned explicitly by PlaceText. Leaving this layout
            // group enabled drives their RectTransforms to zero height at runtime.
            layoutGroup.enabled = false;
        }

        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = InventoryUITheme.PanelSecondary;
        }

        InventoryUITheme.EnsureOutline(gameObject, InventoryUITheme.Border, new Vector2(2f, -2f));

        StyleText(hpText, InventoryUITheme.TextPrimary);
        StyleText(damageText, InventoryUITheme.TextPrimary);
        StyleText(armorText, InventoryUITheme.TextPrimary);
        StyleText(moveSpeedText, InventoryUITheme.TextMuted);
        StyleText(attackSpeedText, InventoryUITheme.TextMuted);
        StyleText(jumpForceText, InventoryUITheme.TextMuted);

        PlaceText(hpText, 0);
        PlaceText(damageText, 1);
        PlaceText(armorText, 2);
        PlaceText(moveSpeedText, 3);
        PlaceText(attackSpeedText, 4);
        PlaceText(jumpForceText, 5);
    }

    private void StyleText(TextMeshProUGUI text, Color color)
    {
        if (text == null)
        {
            return;
        }

        text.color = color;
        text.fontSize = 24f;
        text.fontSizeMin = 16f;
        text.fontSizeMax = 24f;
        text.enableAutoSizing = true;
        text.richText = true;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.raycastTarget = false;
        InventoryUITheme.EnsureShadow(text.gameObject, new Color(0f, 0f, 0f, 0.58f), new Vector2(1f, -1f));
    }

    private void PlaceText(TextMeshProUGUI text, int index)
    {
        if (text == null)
        {
            return;
        }

        RectTransform rect = text.rectTransform;
        float yMax = 0.88f - index * 0.145f;
        float yMin = yMax - 0.105f;
        rect.anchorMin = new Vector2(0f, Mathf.Max(0.02f, yMin));
        rect.anchorMax = new Vector2(1f, Mathf.Max(0.12f, yMax));
        rect.offsetMin = new Vector2(24f, 0f);
        rect.offsetMax = new Vector2(-18f, 0f);
        rect.localScale = Vector3.one;
    }
}
