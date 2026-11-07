using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Shop shop;                 
    [SerializeField] private ShopSlotUI slotPrefab;
    [SerializeField] private Transform slotParent;      
    [SerializeField] private List<ShopItem> items;

    private void Start()
    {
        foreach (var item in items)
        {
            ShopSlotUI slot = Instantiate(slotPrefab, slotParent);
            slot.Setup(item, shop.Buy);  
        }
    }
}