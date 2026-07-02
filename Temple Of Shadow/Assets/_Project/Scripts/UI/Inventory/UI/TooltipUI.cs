using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI descriptionText;

    [SerializeField] private Vector2 screenOffset = new Vector2(-24f, -24f);

    private RectTransform rectTransform;
    private Canvas rootCanvas;
    private CanvasGroup canvasGroup;
    private Camera uiCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        uiCamera = rootCanvas != null ? rootCanvas.worldCamera : null;
        Hide();
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
        if (item == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);

        nameText.text = item.itemName;
        descriptionText.text = item.description;
        statsText.text = BuildStatsText(item);

        FollowMouse();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void FollowMouse()
    {
        if (rectTransform == null)
        {
            return;
        }

        Vector2 mousePosition = Input.mousePosition;

        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransform canvasRect = rootCanvas.transform as RectTransform;
            if (canvasRect != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, uiCamera, out Vector2 localPoint))
            {
                Vector2 offset = screenOffset / Mathf.Max(rootCanvas.scaleFactor, 0.0001f);
                Vector2 desiredPosition = localPoint + offset;
                Vector2 tooltipSize = rectTransform.rect.size;
                Rect canvasBounds = canvasRect.rect;

                float minX = canvasBounds.xMin + (tooltipSize.x * rectTransform.pivot.x);
                float maxX = canvasBounds.xMax - (tooltipSize.x * (1f - rectTransform.pivot.x));
                float minY = canvasBounds.yMin + (tooltipSize.y * rectTransform.pivot.y);
                float maxY = canvasBounds.yMax - (tooltipSize.y * (1f - rectTransform.pivot.y));

                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

                rectTransform.anchoredPosition = desiredPosition;
                return;
            }
        }

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 tooltipScreenSize = rectTransform.rect.size;
        Vector2 pivot = rectTransform.pivot;
        Vector2 targetPosition = mousePosition + screenOffset;

        float minScreenX = tooltipScreenSize.x * pivot.x;
        float maxScreenX = screenSize.x - (tooltipScreenSize.x * (1f - pivot.x));
        float minScreenY = tooltipScreenSize.y * pivot.y;
        float maxScreenY = screenSize.y - (tooltipScreenSize.y * (1f - pivot.y));

        targetPosition.x = Mathf.Clamp(targetPosition.x, minScreenX, maxScreenX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minScreenY, maxScreenY);

        rectTransform.position = targetPosition;
    }

    private string BuildStatsText(ItemData item)
    {
        if (item is not EquipmentData equipment)
        {
            return string.Empty;
        }

        string stats = string.Empty;
        AppendStat(ref stats, "HP", equipment.maxHP);
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

        AppendLine(ref stats, $"{label}: +{value}");
    }

    private void AppendStat(ref string stats, string label, float value)
    {
        if (Mathf.Approximately(value, 0f))
        {
            return;
        }

        AppendLine(ref stats, $"{label}: +{value:0.##}");
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