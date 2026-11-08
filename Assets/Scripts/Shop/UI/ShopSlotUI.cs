using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ShopSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private Image soldOutImage;
    [SerializeField] private Image mysteryImage;  
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] UpgradeChannel hoverChannel;
    [SerializeField] EventChannel hoverExitChannel;

    private bool isEndgameItem;

    private bool IsMystery;

    public ShopItem Item { get; private set; }
    static readonly Color greyedOut = new Color(0.2f, 0.2f, 0.2f, 1f);

    


    
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
        IsMystery = isEndgameItem && !endGame;
        bool greyed = !IsMystery && (!unlocked || soldOut);
        mysteryImage.gameObject.SetActive(IsMystery);
        icon.color = greyed ? greyedOut : Color.white;
        button.interactable = !IsMystery && unlocked && !soldOut && money >= Item.price;
   }

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if (!IsMystery)
            hoverChannel.Call(Item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverExitChannel.Call();
    }
}
