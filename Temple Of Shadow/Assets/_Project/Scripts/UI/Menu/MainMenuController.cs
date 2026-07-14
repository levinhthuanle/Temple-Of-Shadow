using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu1Panel;
    [SerializeField] private GameObject menu2Panel;

    private void Start()
    {
        menu1Panel.SetActive(true);
        menu2Panel.SetActive(false);
    }

    public void OpenMenu2()
    {
        menu1Panel.SetActive(false);
        menu2Panel.SetActive(true);
    }

    public void BackToMenu1()
    {
        menu2Panel.SetActive(false);
        menu1Panel.SetActive(true);
    }
}