using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop", menuName = "Shop/Shop")]
public class Shop : ScriptableObject


{

    public GameManager gameManager;
    // [SerializeField] private ShopCatalog catalog;
    // [SerializeField] private Inventory inventory;
    [SerializeField] private ShopItem item;
    
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

    private void FailPurchase()
    {
        
    }



}
