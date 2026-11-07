using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Shop : MonoBehaviour 


{
    [SerializeField] private GameManager gameManager;

    
    private Dictionary<ShopItem, int> owned = new Dictionary<ShopItem, int>();
    public float Money => gameManager.GetMoney();

    
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
            HandleApproved(item);
        }
    }

    private void HandleApproved(ShopItem item)
    {
        gameManager.UpdateMoney(-item.price);
        owned[item] = GetOwned(item) + 1;
        gameManager.AddClock(item);
        Debug.Log("Item Bought yay");
    }
    private void HandleDenied()
    {

        Debug.Log("Cant buy item");
    }

}
