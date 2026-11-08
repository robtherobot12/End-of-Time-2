using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Float Channel", menuName = "ScriptableObjects/FloatChannel")]
public class FloatChannel : ScriptableObject
{
    public UnityEvent<float> _event;

    public void Call(float f)
    {
        _event?.Invoke(f);
    }
}