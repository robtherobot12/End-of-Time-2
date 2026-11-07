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

    void OnEnable()
    {
        halftimeChannel._event.AddListener(HalftimeSwitch);
    }

    void OnDisable()
    {
        halftimeChannel._event.RemoveListener(HalftimeSwitch);
    }

    void HalftimeSwitch()
    {
        addToShop(5, 9);
    }

    private void Start()
    {
        addToShop(0, 4);
    }

    private void addToShop(int start, int end)
    {
        ShopSlotUI slot = Instantiate(slotPrefab, slotParent);
        slot.Setup(items[start], shop.Buy, true); 
        slots.Add(slot);

        for (int i = start + 1; i <= end; i++)
        {
            slot = Instantiate(slotPrefab, slotParent);
            slot.Setup(items[i], shop.Buy, false); 
            slots.Add(slot);
        }
    }

    private void Update()
    {
        int i = 0;
        foreach (var slot in slots)
        {
            slot.Refresh(shop.GetOwned(slot.Item), shop.Money, i <= shop.lastUnlock);
            i++;
        }
    }
}