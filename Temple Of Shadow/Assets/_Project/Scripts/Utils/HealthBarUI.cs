using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= UpdateHealthBar;
        }
    }

    private void Start()
    {
        RefreshHealthBar();
    }

    private void Update()
    {
        RefreshHealthBar();
    }

    private void RefreshHealthBar()
    {
        if (playerHealth == null || slider == null)
        {
            return;
        }

        UpdateHealthBar(playerHealth.GetCurrentHp(), playerHealth.GetMaxHp());
    }

    private void UpdateHealthBar(int currentHp, int maxHp)
    {
        if (slider == null)
        {
            return;
        }

        slider.maxValue = maxHp;
        slider.value = currentHp;
    }
}
