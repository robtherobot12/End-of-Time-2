using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event Channel", menuName = "ScriptableObjects/Events")]
public class EventChannel : ScriptableObject
{
    public UnityEvent _event;
    
    public void Call()
    {
        _event?.Invoke();
    }
}
