using UnityEngine;
using UnityEngine.EventSystems;

public class ClockMovement : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera cam;
    Vector3 offset;

    void Awake()
    {
        cam = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localPosition = new Vector3(-5,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData e)
    {
        Debug.Log("Clicked " + name);
    }

    public void OnBeginDrag(PointerEventData e)
    {
        Vector3 w = cam.ScreenToWorldPoint(e.position);
        w.z = transform.position.z;
        offset = transform.position - w;
    }

    public void OnDrag(PointerEventData e)
    {
        Vector3 w = cam.ScreenToWorldPoint(e.position);
        w.z = transform.position.z;
        transform.position = w + offset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        Debug.Log("Drag ended");
    }
}
