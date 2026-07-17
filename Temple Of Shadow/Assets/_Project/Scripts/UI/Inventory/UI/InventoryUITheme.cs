using UnityEngine;
using UnityEngine.UI;

public static class InventoryUITheme
{
    public static readonly Color Overlay = new Color(0.006f, 0.007f, 0.011f, 0.88f);
    public static readonly Color Panel = new Color(0.014f, 0.017f, 0.024f, 0.995f);
    public static readonly Color PanelSecondary = new Color(0.026f, 0.032f, 0.044f, 1f);
    public static readonly Color PanelRaised = new Color(0.052f, 0.061f, 0.078f, 1f);
    public static readonly Color Header = new Color(0.045f, 0.052f, 0.068f, 0.99f);
    public static readonly Color SlotEmpty = new Color(0.025f, 0.028f, 0.034f, 0.995f);
    public static readonly Color SlotFilled = new Color(0.060f, 0.068f, 0.086f, 1f);
    public static readonly Color SlotEquipment = new Color(0.19f, 0.135f, 0.075f, 1f);
    public static readonly Color SlotConsumable = new Color(0.055f, 0.145f, 0.095f, 1f);
    public static readonly Color SlotHover = new Color(0.25f, 0.25f, 0.29f, 1f);
    public static readonly Color Border = new Color(0.62f, 0.58f, 0.50f, 0.78f);
    public static readonly Color BorderSoft = new Color(0.30f, 0.32f, 0.38f, 0.92f);
    public static readonly Color Accent = new Color(0.92f, 0.66f, 0.24f, 1f);
    public static readonly Color ConsumableAccent = new Color(0.42f, 0.88f, 0.56f, 1f);
    public static readonly Color EquipmentAccent = new Color(1f, 0.7f, 0.34f, 1f);
    public static readonly Color TextPrimary = new Color(0.98f, 0.95f, 0.88f, 1f);
    public static readonly Color TextMuted = new Color(0.76f, 0.80f, 0.87f, 1f);
    public static readonly Color TextDim = new Color(0.52f, 0.56f, 0.63f, 1f);
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
