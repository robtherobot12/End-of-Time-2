using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Upgrade Channel", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeChannel : ScriptableObject
{
    public UnityEvent<ShopItem> _event;
    
    public void Call(ShopItem item)
    {
        _event?.Invoke(item);
    }
}
