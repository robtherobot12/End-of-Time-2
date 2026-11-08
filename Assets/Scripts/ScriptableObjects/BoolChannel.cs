using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event Channels", menuName = "ScriptableObjects/BoolChannel")]
public class BoolChannel : ScriptableObject
{
    public UnityEvent<bool> _event;

    public void Call(bool b)
    {
        _event?.Invoke(b);
    }
}
