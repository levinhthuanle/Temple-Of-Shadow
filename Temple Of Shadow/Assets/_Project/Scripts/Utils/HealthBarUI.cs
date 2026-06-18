using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.maxValue = playerHealth.GetMaxHp();
        slider.value = playerHealth.GetCurrentHp();
    }

    private void Update()
    {
        slider.value = playerHealth.GetCurrentHp();
    }
}