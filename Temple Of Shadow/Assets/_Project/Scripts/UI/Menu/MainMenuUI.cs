using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SoundManager.Instance?.PlaySFX("click_button");
        MainMenuController controller = FindFirstObjectByType<MainMenuController>();
        if (controller != null)
        {
            controller.OpenMenu2();
            return;
        }

        GameSession.SelectSlot(1);
        SceneManager.LoadScene("CharacterSelect");
    }
}
