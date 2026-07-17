using System.Collections.Generic;
using UnityEngine;

public class ShopDatabase : MonoBehaviour
{
    [Tooltip("Fallback list used by a generic shop.")]
    public List<ItemData> itemsForSale = new();

    [Header("Left side - Weapons")]
    public List<ItemData> weaponsForSale = new();

    [Header("Right side - Potions and Keys")]
    public List<ItemData> utilityItemsForSale = new();
}
