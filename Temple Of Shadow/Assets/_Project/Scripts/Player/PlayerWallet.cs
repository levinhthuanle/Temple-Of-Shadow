using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private CoinUI coinUI;

    public int Gold { get; private set; }

    private void Start()
    {
        coinUI.UpdateCoin(Gold);
    }

    public void AddGold(int amount)
    {
        Gold += amount;

        coinUI.UpdateCoin(Gold);
    }
}