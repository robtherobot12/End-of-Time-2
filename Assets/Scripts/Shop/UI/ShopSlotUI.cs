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
    [SerializeField] private Image mysteryImage;  
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    private bool isEndgameItem;

    public ShopItem Item { get; private set; }


    
    public void Setup(ShopItem item, Action<ShopItem> onClick, bool isEndgameItem)
    {
        Item = item;
        this.isEndgameItem = isEndgameItem;
        nameText.text = item.displayName;
        priceText.text = item.price.ToString();
        icon.sprite = item.icon;
        icon.enabled = item.icon != null;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(Item));
    }

    public void Refresh(float money, bool unlocked, bool endGame, bool soldOut)
    {
        bool mystery = isEndgameItem && !endGame;
        mysteryImage.enabled = mystery;
        soldOutImage.enabled = !mystery && (!unlocked || soldOut); 
        button.interactable = !mystery && unlocked && !soldOut && money >= Item.price;
    }
}
