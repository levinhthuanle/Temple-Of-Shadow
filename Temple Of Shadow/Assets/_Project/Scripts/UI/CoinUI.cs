using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    public void UpdateCoin(int amount)
    {
        coinText.text = $"Gold: {amount}";
    }
}