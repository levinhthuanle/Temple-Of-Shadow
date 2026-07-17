using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private CoinUI coinUI;
    [SerializeField, Min(0)] private int startingGold;

    public int Gold { get; private set; }

    public event System.Action<int> GoldChanged;

    private void Awake()
    {
        Gold = startingGold;
    }

    private void Start()
    {
        ResolveCoinUI();
        RefreshUI();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        Gold += amount;

        ResolveCoinUI();
        NotifyGoldChanged();
    }

    private void ResolveCoinUI()
    {
        if (coinUI == null)
        {
            coinUI = FindAnyObjectByType<CoinUI>(FindObjectsInactive.Include);
        }
    }

    public bool CanAfford(int amount)
    {
        return amount >= 0 && Gold >= amount;
    }

    public bool TrySpendGold(int amount)
    {
        if (!CanAfford(amount))
            return false;

        Gold -= amount;
        NotifyGoldChanged();
        return true;
    }

    private void NotifyGoldChanged()
    {
        RefreshUI();
        GoldChanged?.Invoke(Gold);
    }

    private void RefreshUI()
    {
        if (coinUI != null)
            coinUI.UpdateCoin(Gold);
    }
}
