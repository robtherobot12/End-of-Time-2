using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class Shop : MonoBehaviour 


{
    [SerializeField] private GameManager gameManager;
    [SerializeField] UpgradeChannel upgradeChannel;
    [SerializeField] EventChannel halftimeChannel;
    public List<ShopItem> Items = new List<ShopItem>();
    public void SetItems(List<ShopItem> list) => Items = list;
    public const int EndgameStart = 5;
    public int lastUnlock = 0;
    public bool EndGame = false;

    public bool IsUnlocked(int index)
    {
        return index <= lastUnlock && (index < EndgameStart || EndGame);
    }

    public bool IsSoldOut(ShopItem item)
    {
        // 0 = unlimited
        return item.maxOwned > 0 && GetOwned(item) >= item.maxOwned;   
    }

    
    private Dictionary<ShopItem, int> owned = new Dictionary<ShopItem, int>();

    public float Money => gameManager.GetMoney();


    
    public int GetOwned(ShopItem item)
    {
        return owned.TryGetValue(item, out int count) ? count : 0;
    }

    public void Buy(ShopItem item)
    {
       int idx = Items.IndexOf(item);
        if (Money < item.price || !IsUnlocked(idx) || IsSoldOut(item)) { 
            HandleDenied(); 
            return; 
        }
        upgradeChannel.Call(item); 
    }

    void OnEnable()
    {
        upgradeChannel._event.AddListener(HandleApproved);
        halftimeChannel._event.AddListener(HalftimeSwitch);
    }

    void OnDisable()
    {
        upgradeChannel._event.RemoveListener(HandleApproved);
        halftimeChannel._event.RemoveListener(HalftimeSwitch);
    }

    private void HandleApproved(ShopItem item)
    {
        owned[item] = GetOwned(item) + 1;
        int idx = Items.IndexOf(item);
        if (idx == lastUnlock) lastUnlock++;
        Debug.Log("Item Bought yay");
    }
    private void HandleDenied()
    {
        Debug.Log("Cant buy item");
    }

    void HalftimeSwitch()
    {
        EndGame = true;
    }

}
