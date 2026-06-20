using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        // Played through the persistent SoundManager so it survives the scene change.
        SoundManager.Instance?.PlaySFX("click_button");
        SceneManager.LoadScene("SaveSlotSelect");
    }
}