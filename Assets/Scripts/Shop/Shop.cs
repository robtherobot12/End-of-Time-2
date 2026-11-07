using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour 


{
    [SerializeField] private GameManager gameManager;
  
    public void Buy(ShopItem item)
    {

        if (gameManager.GetMoney() < item.price)
        {
            HandleDenied();
            return;
        }
        else
        {
            HandleApproved(item);
        }
    }

    private void HandleApproved(ShopItem item)
    {
        gameManager.UpdateMoney(-item.price);
    }
    private void HandleDenied()
    {
        
    }

}
