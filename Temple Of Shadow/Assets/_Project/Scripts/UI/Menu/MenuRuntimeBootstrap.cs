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

        GameObject bridgeObject = new("GameplayProfileBridge");
        bridgeObject.AddComponent<GameplayProfileBridge>();
    }
}
