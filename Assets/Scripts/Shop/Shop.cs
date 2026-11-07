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

        if (gameManager.GetMoney() < item.getPrice())
        {
            HandleDenied();
            return;
        }
        else
        {
            HandleApproved();
        }
    }

    private void HandleApproved()
    {
        gameManager.UpdateMoney(item.getPrice());
    }
    private void HandleDenied()
    {
        
    }

    private void FailPurchase()
    {
        
    }



}
