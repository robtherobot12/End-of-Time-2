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
    private bool EndGame = false;

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
        EndGame = true;
    }

    private void Start()
    {
        addToShop(0, 9);
    }

    private void addToShop(int start, int end)
    {
        ShopSlotUI slot = Instantiate(slotPrefab, slotParent);
        slot.Setup(items[start], shop.Buy, true, EndGame); 
        slots.Add(slot);

        for (int i = start + 1; i <= 4; i++)
        {
            slot = Instantiate(slotPrefab, slotParent);
            slot.Setup(items[i], shop.Buy, false, EndGame); 
            slots.Add(slot);
        }

        for (int i = 5; i <= end; i++)
        {
            slot = Instantiate(slotPrefab, slotParent);
            slot.Setup(items[i], shop.Buy, false, EndGame); 
            slots.Add(slot);
        }
    }

    private void Update()
    {
        int i = 0;
        foreach (var slot in slots)
        {
            slot.Refresh(shop.GetOwned(slot.Item), shop.Money, i <= shop.lastUnlock, EndGame);
            i++;
        }
    }
}