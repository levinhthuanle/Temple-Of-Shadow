using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Transform fill;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Awake()
    {
        originalScale = fill.localScale;
        originalPosition = fill.localPosition;
    }

    public void UpdateHealthBar(int currentHp, int maxHp)
    {

        float healthPercent = (float)currentHp / maxHp;
        healthPercent = Mathf.Clamp01(healthPercent);

        fill.localScale = new Vector3(
            originalScale.x * healthPercent,
            originalScale.y,
            originalScale.z
        );

        float lostWidth = originalScale.x * (1 - healthPercent);
        fill.localPosition = new Vector3(
            originalPosition.x - lostWidth / 2f,
            originalPosition.y,
            originalPosition.z
        );
    }
}