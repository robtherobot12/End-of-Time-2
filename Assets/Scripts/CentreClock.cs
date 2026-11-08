using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CentreClock : MonoBehaviour, IPointerClickHandler    
{
    GameManager gm;

    void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }
    
    public void OnPointerClick(PointerEventData e)
    {
              gm.ClickClock();
    }
}
