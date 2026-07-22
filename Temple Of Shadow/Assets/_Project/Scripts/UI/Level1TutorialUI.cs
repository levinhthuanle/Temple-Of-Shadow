using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level1TutorialUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private PlayerController playerController;
    private bool tutorialActive;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Level1")
        {
            enabled = false;
            return;
        }

        BuildTutorial();
    }

    private void Start()
    {
        ShowTutorial();
    }

    private void Update()
    {
        if (tutorialActive &&
            (Input.anyKeyDown ||
             Input.GetMouseButtonDown(0) ||
             Input.GetMouseButtonDown(1) ||
             Input.GetMouseButtonDown(2)))
        {
            CompleteTutorial();
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
    }

    private void BuildTutorial()
    {
        Transform existing = transform.Find("Level1TutorialPanel");
        GameObject panel = existing != null
            ? existing.gameObject
            : new GameObject("Level1TutorialPanel", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        panel.transform.SetAsLastSibling();

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -24f);
        panelRect.sizeDelta = new Vector2(840f, 300f);

        Image background = panel.GetComponent<Image>();
        if (background == null) background = panel.AddComponent<Image>();
        background.color = new Color(0.035f, 0.025f, 0.055f, 0.94f);
        background.raycastTarget = true;

        Outline outline = panel.GetComponent<Outline>();
        if (outline == null) outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.62f, 0.32f, 0.88f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);

        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        TextMeshProUGUI title = CreateText(panel.transform, "Title", new Vector2(0f, -22f), new Vector2(790f, 48f), 30f);
        title.text = "HOW TO PLAY";
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(0.9f, 0.72f, 1f);

        TextMeshProUGUI controls = CreateText(panel.transform, "Controls", new Vector2(0f, -78f), new Vector2(790f, 164f), 22f);
        controls.text =
            "<color=#E9C8FF><b>A / D</b></color> or <color=#E9C8FF><b>LEFT / RIGHT</b></color>  Move\n" +
            "<color=#E9C8FF><b>W</b></color> or <color=#E9C8FF><b>SPACE</b></color>  Jump  •  Press again in midair to Double Jump\n" +
            "<color=#E9C8FF><b>J</b></color>  Slash       <color=#E9C8FF><b>K</b></color>  Throw Projectile       <color=#E9C8FF><b>L</b></color>  Kick";
        controls.alignment = TextAlignmentOptions.Center;
        controls.lineSpacing = 12f;

        TextMeshProUGUI footer = CreateText(panel.transform, "Footer", new Vector2(0f, -250f), new Vector2(790f, 30f), 18f);
        footer.text = "PRESS ANY KEY OR CLICK TO CONTINUE";
        footer.fontStyle = FontStyles.Bold;
        footer.color = new Color(1f, 0.84f, 0.38f);
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, Vector2 position, Vector2 size, float fontSize)
    {
        Transform existing = parent.Find(name);
        GameObject textObject = existing != null
            ? existing.gameObject
            : new GameObject(name, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        if (text == null) text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private void ShowTutorial()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.SetCanMove(false);
        }

        tutorialActive = true;
    }

    private void CompleteTutorial()
    {
        tutorialActive = false;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        StartCoroutine(UnlockPlayerNextFrame());
    }

    private System.Collections.IEnumerator UnlockPlayerNextFrame()
    {
        yield return null;

        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
    }
}
