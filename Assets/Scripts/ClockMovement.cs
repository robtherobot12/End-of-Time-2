using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockMovement : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera cam;
    Vector3 offset;
    float x_offset, y_offset;
    float x_speed, y_speed = 0.3f;

    void Awake()
    {
        cam = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localPosition = new Vector3(-5,0,0);
        StartCoroutine(Float());
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

    IEnumerator Float()
    {
        while (true)
        {
            float t = 0;
            
            while (t < 1)
            {
                yield return new WaitForEndOfFrame();
                transform.localPosition = offset + new Vector3(x_offset, y_offset, 0);
                x_offset = Mathf.Sin(t);
                y_offset = Mathf.Lerp(-1, 1, t);
                t += Time.deltaTime;
            }

            while (t > 0)
            {
                yield return new WaitForEndOfFrame();
                transform.localPosition = offset + new Vector3(x_offset, y_offset, 0);
                x_offset = Mathf.Lerp(-1, 1, t);
                y_offset = Mathf.Lerp(-1, 1, t);
                t -= Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
