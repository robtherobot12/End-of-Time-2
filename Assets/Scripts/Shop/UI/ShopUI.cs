using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Shop shop;                 
    [SerializeField] private ShopSlotUI slotPrefab;
    [SerializeField] private Transform slotParent;      
    [SerializeField] private List<ShopItem> items;
    [SerializeField] EventChannel halftimeChannel;

    
    private List<ShopSlotUI> slots = new List<ShopSlotUI>();

    private void Awake()
    {
        shop.SetItems(items);
    }

    private void Start()
    {
        for (int i = 0; i < shop.Items.Count; i++)
        {
            var slot = Instantiate(slotPrefab, slotParent);
            slot.Setup(shop.Items[i], shop.Buy, isEndgameItem: i >= Shop.EndgameStart);
            slots.Add(slot);
        }
    }

    private void Update()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].Refresh(shop.Money, shop.IsUnlocked(i), shop.EndGame, shop.IsSoldOut(shop.Items[i]));
    }
}