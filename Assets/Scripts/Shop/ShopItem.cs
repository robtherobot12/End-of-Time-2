using Unity.VisualScripting;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private float price;

    public float getPrice()
    {
        return price;
    }
}