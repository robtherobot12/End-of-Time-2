using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    public float pulseRate;
    public float pulseAmount;
    public float price;
    public int maxOwned;
    public string displayName;
    public string description;
    public Sprite icon;
    public Sprite soldOut;
    public GameObject prefab;


}