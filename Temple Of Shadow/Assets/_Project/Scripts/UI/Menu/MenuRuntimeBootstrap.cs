using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuRuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneHooks()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        if (scene.name == "CharacterSelect")
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null && canvas.GetComponent<PreparationMenuController>() == null)
            {
                canvas.gameObject.AddComponent<PreparationMenuController>();
            }

            return;
        }

        GameContentCatalog catalog = GameContentCatalog.Load();
        if (catalog == null || catalog.GetLevelByScene(scene.name) == null)
        {
            return;
        }

        if (Object.FindFirstObjectByType<GameplayProfileBridge>() == null)
        {
            GameObject bridgeObject = new("GameplayProfileBridge");
            bridgeObject.AddComponent<GameplayProfileBridge>();
        }

        if (Object.FindFirstObjectByType<PauseMenuController>() == null)
        {
            GameObject pauseObject = new("PauseMenuController");
            pauseObject.AddComponent<PauseMenuController>();
        }
    }
}
