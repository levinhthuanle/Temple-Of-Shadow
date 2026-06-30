using TMPro;
using UnityEngine;

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
        Refresh();
    }

    private void OnEnable()
    {
        ResolveReferences();
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

        SetText(hpText, $"HP: {currentHp}/{maxHp}");
        SetText(damageText, $"Damage: {playerStats.Damage}");
        SetText(armorText, $"Armor: {playerStats.Armor}");
        SetText(moveSpeedText, $"Speed: {FormatFloat(playerStats.MoveSpeed)}");
        SetText(attackSpeedText, $"Attack Speed: {FormatFloat(playerStats.AttackSpeed)}");
        SetText(jumpForceText, $"Jump Force: {FormatFloat(playerStats.JumpForce)}");
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

        hpText ??= FindText("HPText");
        damageText ??= FindText("DamageText");
        armorText ??= FindText("ArmorText");
        moveSpeedText ??= FindText("SpeedText");
        attackSpeedText ??= FindText("AttackSpeedText");
        jumpForceText ??= FindText("JumpForceText");

        if (damageText == null)
        {
            damageText = CreateText("DamageText", 1);
        }
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
        SetText(hpText, $"HP: {currentHp}/{maxHp}");
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
}
