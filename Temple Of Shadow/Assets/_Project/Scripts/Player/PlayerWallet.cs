using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private CoinUI coinUI;

    public int Gold { get; private set; }

    private void Start()
    {
        ResolveCoinUI();
        coinUI?.UpdateCoin(Gold);
    }

    public void AddGold(int amount)
    {
        Gold += amount;

        ResolveCoinUI();
        coinUI?.UpdateCoin(Gold);
    }

    private void ResolveCoinUI()
    {
        if (coinUI == null)
        {
            coinUI = FindAnyObjectByType<CoinUI>(FindObjectsInactive.Include);
        }
    }
}
