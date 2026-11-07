using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event Channel", menuName = "ScriptableObjects/Events")]
public class UpgradeChannel : ScriptableObject
{
    public UnityEvent<ShopItem> _event;
    
    public void Call(ShopItem item)
    {
        _event?.Invoke(item);
    }
}
