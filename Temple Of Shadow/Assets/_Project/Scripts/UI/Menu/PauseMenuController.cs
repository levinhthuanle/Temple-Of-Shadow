using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    private static readonly Color OverlayColor = new(0.006f, 0.008f, 0.013f, 0.88f);
    private static readonly Color PanelColor = new(0.016f, 0.020f, 0.028f, 0.99f);
    private static readonly Color ButtonColor = new(0.034f, 0.040f, 0.052f, 1f);
    private static readonly Color BorderColor = new(0.54f, 0.58f, 0.65f, 0.72f);
    private static readonly Color GoldColor = new(0.72f, 0.42f, 0.10f, 1f);
    private static readonly Color TextColor = new(0.97f, 0.95f, 0.88f, 1f);
    private static readonly Color MutedTextColor = new(0.76f, 0.80f, 0.87f, 1f);

    private GameObject pauseRoot;
    private Button resumeButton;
    private bool isPaused;
    private CursorLockMode previousLockMode;
    private bool previousCursorVisible;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (FindObjectsByType<PauseMenuController>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        BuildInterface();
        SetPaused(false, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnDisable()
    {
        if (isPaused)
        {
            RestoreGameState();
        }
    }

    private void OnDestroy()
    {
        if (isPaused)
        {
            RestoreGameState();
        }
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void ResumeGame()
    {
        PlayClick();
        SetPaused(false);
    }

    public void RestartLevel()
    {
        PlayClick();
        RestoreGameState();
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.buildIndex >= 0)
        {
            SceneManager.LoadScene(activeScene.buildIndex);
        }
        else
        {
            SceneManager.LoadScene(activeScene.name);
        }
    }

    public void ReturnToMainMenu()
    {
        PlayClick();
        RestoreGameState();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        PlayClick();
        RestoreGameState();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetPaused(bool paused, bool rememberCursor = true)
    {
        if (pauseRoot == null)
        {
            return;
        }

        if (paused && rememberCursor)
        {
            previousLockMode = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
        }

        isPaused = paused;
        pauseRoot.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            EventSystem.current?.SetSelectedGameObject(resumeButton != null ? resumeButton.gameObject : null);
        }
        else if (rememberCursor)
        {
            Cursor.lockState = previousLockMode;
            Cursor.visible = previousCursorVisible;
            EventSystem.current?.SetSelectedGameObject(null);
        }
    }

    private void RestoreGameState()
    {
        Time.timeScale = 1f;
        Cursor.lockState = previousLockMode;
        Cursor.visible = previousCursorVisible;
        isPaused = false;
    }

    private void BuildInterface()
    {
        EnsureEventSystem();

        GameObject canvasObject = new("PauseMenuCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        pauseRoot = CreatePanel("PauseOverlay", canvasObject.transform, OverlayColor).gameObject;
        StretchToParent(pauseRoot.GetComponent<RectTransform>());

        RectTransform window = CreatePanel("PauseWindow", pauseRoot.transform, PanelColor);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.sizeDelta = new Vector2(520f, 650f);
        window.anchoredPosition = Vector2.zero;

        Outline windowOutline = window.gameObject.AddComponent<Outline>();
        windowOutline.effectColor = BorderColor;
        windowOutline.effectDistance = new Vector2(2f, -2f);
        windowOutline.useGraphicAlpha = false;

        Shadow windowShadow = window.gameObject.AddComponent<Shadow>();
        windowShadow.effectColor = new Color(0f, 0f, 0f, 0.62f);
        windowShadow.effectDistance = new Vector2(12f, -12f);

        VerticalLayoutGroup layout = window.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(54, 54, 48, 48);
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateText(window, "PAUSED", 44f, FontStyles.Bold, TextAlignmentOptions.Center, TextColor, 72f);
        CreateText(window, SceneManager.GetActiveScene().name.ToUpperInvariant(), 18f, FontStyles.Bold,
            TextAlignmentOptions.Center, MutedTextColor, 36f);

        CreateSpacer(window, 16f);
        resumeButton = CreateButton(window, "CONTINUE", ResumeGame, true);
        CreateButton(window, "RESTART LEVEL", RestartLevel);
        CreateButton(window, "MAIN MENU", ReturnToMainMenu);
        CreateButton(window, "QUIT GAME", QuitGame);
        CreateSpacer(window, 8f);
        CreateText(window, "ESC  •  CLOSE MENU", 15f, FontStyles.Normal, TextAlignmentOptions.Center,
            MutedTextColor, 30f);
    }

    private static RectTransform CreatePanel(string objectName, Transform parent, Color color)
    {
        GameObject panelObject = new(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(parent, false);
        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        return panelObject.GetComponent<RectTransform>();
    }

    private static TMP_Text CreateText(
        Transform parent,
        string value,
        float fontSize,
        FontStyles style,
        TextAlignmentOptions alignment,
        Color color,
        float preferredHeight)
    {
        GameObject textObject = new("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(LayoutElement));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = color;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(12f, fontSize * 0.66f);
        text.fontSizeMax = fontSize;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;
        text.outlineWidth = 0.08f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.68f);

        LayoutElement element = textObject.GetComponent<LayoutElement>();
        element.preferredHeight = preferredHeight;
        return text;
    }

    private static Button CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, bool primary = false)
    {
        GameObject buttonObject = new(label.Replace(" ", string.Empty) + "Button",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = primary ? GoldColor : ButtonColor;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = primary ? new Color(0.96f, 0.73f, 0.31f, 0.9f) : BorderColor;
        outline.effectDistance = new Vector2(2f, -2f);
        outline.useGraphicAlpha = false;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.14f, 1.14f, 1.14f, 1f);
        colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        LayoutElement element = buttonObject.GetComponent<LayoutElement>();
        element.preferredHeight = 76f;

        TMP_Text text = CreateText(buttonObject.transform, label, 22f, FontStyles.Bold,
            TextAlignmentOptions.Center, TextColor, 76f);
        StretchToParent(text.rectTransform);

        return button;
    }

    private static void CreateSpacer(Transform parent, float height)
    {
        GameObject spacer = new("Spacer", typeof(RectTransform), typeof(LayoutElement));
        spacer.transform.SetParent(parent, false);
        spacer.GetComponent<LayoutElement>().preferredHeight = height;
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        GameObject eventSystemObject = new("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        DontDestroyOnLoad(eventSystemObject);
    }

    private static void PlayClick()
    {
        SoundManager.Instance?.PlaySFX("click_button");
    }
}
