using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopNavigationController : MonoBehaviour
{
    private void OnGUI()
    {
        GUIStyle button = new(GUI.skin.button) { fontSize = 18, fontStyle = FontStyle.Bold };
        if (GUI.Button(new Rect(24f, 24f, 190f, 48f), "BACK TO STAGE MAP", button))
        {
            SceneManager.LoadScene("CharacterSelect");
        }
    }
}