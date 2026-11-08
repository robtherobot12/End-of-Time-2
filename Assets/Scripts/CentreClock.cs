using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CentreClock : MonoBehaviour, IPointerClickHandler    
{
    GameManager gm;
    public EventChannel clickClockSfx;

    void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }
    
    public void OnPointerClick(PointerEventData e)
    {
        clickClockSfx.Call();
              gm.ClickClock();
    }
}
