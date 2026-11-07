using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class Shop : MonoBehaviour 


{
    [SerializeField] private GameManager gameManager;
    [SerializeField] UpgradeChannel upgradeChannel;
    [SerializeField] EventChannel halftimeChannel;

    
    private Dictionary<ShopItem, int> owned = new Dictionary<ShopItem, int>();

    public float Money => gameManager.GetMoney();

    public int lastUnlock = 0;

    
    public int GetOwned(ShopItem item)
    {
        return owned.TryGetValue(item, out int count) ? count : 0;
    }

    public void Buy(ShopItem item)
    {
        if (Money < item.price || GetOwned(item) >= item.maxOwned)
        {
            // never really gets called but is a safety check.
            HandleDenied();
        }
        else
        {
            //HandleApproved(item);
            upgradeChannel.Call(item);
        }
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
        
        Debug.Log("Item Bought yay");
    }
    private void HandleDenied()
    {

        Debug.Log("Cant buy item");
    }

    void HalftimeSwitch()
    {
        // change stuff
    }

}
