using UnityEngine;

public class ShopZoneController : MonoBehaviour
{
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ShopDatabase database;
    [SerializeField] private Transform player;
    [SerializeField] private float leftShopBoundary = -4f;
    [SerializeField] private float rightShopBoundary = 4f;

    private int activeZone;

    private void Update()
    {
        shopUI ??= FindAnyObjectByType<ShopUI>();
        database ??= FindAnyObjectByType<ShopDatabase>();
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject != null ? playerObject.transform : null;
        }

        if (shopUI == null || database == null || player == null)
            return;

        int newZone = player.position.x <= leftShopBoundary ? -1 : player.position.x >= rightShopBoundary ? 1 : 0;
        if (newZone == activeZone)
            return;

        activeZone = newZone;
        if (activeZone < 0)
            shopUI.OpenShop("WEAPON FORGE", database.weaponsForSale, false);
        else if (activeZone > 0)
            shopUI.OpenShop("POTIONS & KEYS", database.utilityItemsForSale, true);
        else
            shopUI.CloseShop();
    }
}
