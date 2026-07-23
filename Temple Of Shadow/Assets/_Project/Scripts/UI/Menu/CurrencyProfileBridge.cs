using UnityEngine;

public class CurrencyProfileBridge : MonoBehaviour
{
    private PlayerWallet wallet;
    private bool applying;

    private void Start()
    {
        GameProfileData profile = GameSession.EnsureProfile();
        wallet = FindFirstObjectByType<PlayerWallet>();

        if (wallet == null || profile == null)
        {
            return;
        }

        applying = true;
        wallet.SetGold(profile.gold);
        applying = false;
        wallet.GoldChanged += SyncGold;
    }

    private void OnDestroy()
    {
        if (wallet != null)
        {
            wallet.GoldChanged -= SyncGold;
        }
    }

    private void SyncGold(int gold)
    {
        if (applying || GameSession.CurrentProfile == null)
        {
            return;
        }

        GameSession.CurrentProfile.gold = gold;
        GameSession.Save();
    }
}