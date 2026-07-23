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

        bool isShop = scene.name == "Shop";
        GameContentCatalog catalog = GameContentCatalog.Load();
        bool isGameplayLevel = catalog != null && catalog.GetLevelByScene(scene.name) != null;

        if (!isGameplayLevel && !isShop)
        {
            return;
        }

        if (Object.FindFirstObjectByType<GameplayProfileBridge>() == null)
        {
            GameObject bridgeObject = new("GameplayProfileBridge");
            bridgeObject.AddComponent<GameplayProfileBridge>();
        }

        if (Object.FindFirstObjectByType<CurrencyProfileBridge>() == null)
        {
            GameObject currencyObject = new("CurrencyProfileBridge");
            currencyObject.AddComponent<CurrencyProfileBridge>();
        }

        if (isGameplayLevel && Object.FindFirstObjectByType<PauseMenuController>() == null)
        {
            GameObject pauseObject = new("PauseMenuController");
            pauseObject.AddComponent<PauseMenuController>();
        }

        if (isShop && Object.FindFirstObjectByType<ShopNavigationController>() == null)
        {
            GameObject shopNavigationObject = new("ShopNavigationController");
            shopNavigationObject.AddComponent<ShopNavigationController>();
        }
    }
}
