using UnityEngine;
using UnityEngine.EventSystems;

public class IndicatorTester : MonoBehaviour, IPointerClickHandler
{
    public EventChannel indicateChannel;

    public void OnPointerClick(PointerEventData e)
    {
        indicateChannel.Call();
    }



}
