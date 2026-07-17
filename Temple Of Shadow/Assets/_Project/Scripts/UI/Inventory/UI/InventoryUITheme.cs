using UnityEngine;
using UnityEngine.UI;

public static class InventoryUITheme
{
    public static readonly Color Overlay = new Color(0.02f, 0.018f, 0.025f, 0.72f);
    public static readonly Color Panel = new Color(0.075f, 0.078f, 0.095f, 0.97f);
    public static readonly Color PanelSecondary = new Color(0.115f, 0.12f, 0.145f, 0.98f);
    public static readonly Color PanelRaised = new Color(0.155f, 0.16f, 0.19f, 0.98f);
    public static readonly Color Header = new Color(0.18f, 0.18f, 0.21f, 0.96f);
    public static readonly Color SlotEmpty = new Color(0.095f, 0.1f, 0.12f, 0.98f);
    public static readonly Color SlotFilled = new Color(0.16f, 0.165f, 0.19f, 0.99f);
    public static readonly Color SlotEquipment = new Color(0.22f, 0.17f, 0.105f, 0.99f);
    public static readonly Color SlotConsumable = new Color(0.09f, 0.18f, 0.13f, 0.99f);
    public static readonly Color SlotHover = new Color(0.25f, 0.25f, 0.29f, 1f);
    public static readonly Color Border = new Color(0.43f, 0.4f, 0.48f, 0.9f);
    public static readonly Color BorderSoft = new Color(0.22f, 0.225f, 0.27f, 0.95f);
    public static readonly Color Accent = new Color(0.95f, 0.72f, 0.31f, 1f);
    public static readonly Color ConsumableAccent = new Color(0.42f, 0.88f, 0.56f, 1f);
    public static readonly Color EquipmentAccent = new Color(1f, 0.7f, 0.34f, 1f);
    public static readonly Color TextPrimary = new Color(0.97f, 0.96f, 0.91f, 1f);
    public static readonly Color TextMuted = new Color(0.7f, 0.71f, 0.76f, 1f);
    public static readonly Color TextDim = new Color(0.48f, 0.5f, 0.56f, 1f);
    public static readonly Color Positive = new Color(0.48f, 0.9f, 0.58f, 1f);

    public static Image EnsureImage(GameObject target)
    {
        Image image = target.GetComponent<Image>();
        if (image == null)
        {
            image = target.AddComponent<Image>();
        }

        image.raycastTarget = true;
        return image;
    }

    public static LayoutElement EnsureLayoutElement(GameObject target)
    {
        LayoutElement layoutElement = target.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = target.AddComponent<LayoutElement>();
        }

        return layoutElement;
    }

    public static Outline EnsureOutline(GameObject target, Color color, Vector2 distance)
    {
        Outline outline = target.GetComponent<Outline>();
        if (outline == null)
        {
            outline = target.AddComponent<Outline>();
        }

        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = false;
        return outline;
    }

    public static Shadow EnsureShadow(GameObject target, Color color, Vector2 distance)
    {
        Shadow shadow = null;
        Shadow[] shadows = target.GetComponents<Shadow>();
        foreach (Shadow candidate in shadows)
        {
            if (candidate.GetType() == typeof(Shadow))
            {
                shadow = candidate;
                break;
            }
        }

        if (shadow == null)
        {
            shadow = target.AddComponent<Shadow>();
        }

        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
        return shadow;
    }
}
