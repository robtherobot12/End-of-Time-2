using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopSlotUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private Image soldOutImage; 
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    public ShopItem Item { get; private set; }
    
    public void Setup(ShopItem item, Action<ShopItem> onClick)
    {
        Item = item;

        nameText.text = item.displayName;
        priceText.text = item.price.ToString();
        icon.sprite = item.icon;
        icon.enabled = item.icon != null;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(Item));
    }

    public void Refresh(int owned, float money)
    {
        bool soldOut = owned >= Item.maxOwned;
        button.interactable = !soldOut && money >= Item.price;
        soldOutImage.enabled = soldOut;
    }
}
