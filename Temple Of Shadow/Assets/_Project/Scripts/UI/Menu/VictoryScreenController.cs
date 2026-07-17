using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreenController : MonoBehaviour
{
    private static VictoryScreenController instance;

    private int rewardGold;
    private string levelName;
    private string nextSceneName;
    private bool hasNextLevel;
    private bool visible;

    public static void CompleteLevel(int reward, string goalLabel)
    {
        if (instance == null)
        {
            GameObject controllerObject = new("VictoryScreenController");
            instance = controllerObject.AddComponent<VictoryScreenController>();
        }

        instance.Show(reward, goalLabel);
    }

    private void Show(int reward, string goalLabel)
    {
        if (visible)
        {
            return;
        }

        visible = true;
        rewardGold = reward;
        levelName = SceneManager.GetActiveScene().name;

        GameProfileData profile = GameSession.EnsureProfile();
        GameContentCatalog catalog = GameSession.Catalog ?? GameContentCatalog.Load();
        LevelMenuEntry current = catalog != null ? catalog.GetLevelByScene(levelName) : null;
        LevelMenuEntry next = FindNextLevel(catalog, current);

        if (profile != null)
        {
            profile.gold += rewardGold;

            if (next != null && !profile.IsLevelUnlocked(next.Id))
            {
                profile.unlockedLevelIds.Add(next.Id);
            }

            GameSession.Save();
        }

        PlayerWallet wallet = FindFirstObjectByType<PlayerWallet>();
        if (wallet != null && profile != null)
        {
            wallet.SetGold(profile.gold);
        }

        hasNextLevel = next != null;
        nextSceneName = next != null ? next.sceneName : string.Empty;
        Time.timeScale = 0f;
    }

    private static LevelMenuEntry FindNextLevel(GameContentCatalog catalog, LevelMenuEntry current)
    {
        if (catalog == null || current == null || catalog.levels == null)
        {
            return null;
        }

        for (int i = 0; i < catalog.levels.Length - 1; i++)
        {
            if (catalog.levels[i] == current)
            {
                return catalog.levels[i + 1];
            }
        }

        return null;
    }

    private void OnGUI()
    {
        if (!visible)
        {
            return;
        }

        float scale = Mathf.Min(Screen.width / 1920f, Screen.height / 1080f);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1f));

        Rect dim = new(0f, 0f, Screen.width / scale, Screen.height / scale);
        GUI.Box(dim, GUIContent.none);

        Rect panel = new((1920f - 620f) * 0.5f, (1080f - 520f) * 0.5f, 620f, 520f);
        GUI.Box(panel, "VICTORY");

        GUIStyle title = new(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 46, fontStyle = FontStyle.Bold };
        GUIStyle subtitle = new(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 24 };
        GUIStyle button = new(GUI.skin.button) { fontSize = 24, fontStyle = FontStyle.Bold };

        GUI.Label(new Rect(panel.x + 30f, panel.y + 70f, panel.width - 60f, 70f), levelName.ToUpperInvariant() + " COMPLETE", title);
        GUI.Label(new Rect(panel.x + 30f, panel.y + 145f, panel.width - 60f, 45f), $"+ {rewardGold} GOLD", subtitle);
        GUI.Label(new Rect(panel.x + 30f, panel.y + 190f, panel.width - 60f, 35f), "Your progress has been saved.", subtitle);

        float x = panel.x + 90f;
        float width = panel.width - 180f;
        if (hasNextLevel && GUI.Button(new Rect(x, panel.y + 250f, width, 56f), "NEXT LEVEL", button))
        {
            ResumeAndLoad(nextSceneName);
        }

        float firstY = hasNextLevel ? panel.y + 320f : panel.y + 250f;
        if (GUI.Button(new Rect(x, firstY, width, 56f), "VISIT SHOP", button))
        {
            ResumeAndLoad("Shop");
        }

        if (GUI.Button(new Rect(x, firstY + 68f, width, 56f), "PREPARE", button))
        {
            ResumeAndLoad("CharacterSelect");
        }

        if (GUI.Button(new Rect(x, firstY + 136f, width, 56f), "MAIN MENU", button))
        {
            ResumeAndLoad("MainMenu");
        }
    }

    private static void ResumeAndLoad(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}