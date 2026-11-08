using UnityEngine;
using UnityEngine.InputSystem;

public class IndicatorController : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public FloatChannel indicatorEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()  
    { 
        indicatorEvent._event.AddListener(CreateIndicator); 
    }
    void OnDisable() 
    { 
        indicatorEvent._event.RemoveListener(CreateIndicator); 
    }

    public void CreateIndicator(float amount)
    {
        Vector2 pos = Mouse.current.position.ReadValue();

        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform, pos, cam, out Vector2 local);

        GameObject indicator = Instantiate(indicatorPrefab, transform);
        indicator.transform.localPosition = local;
        indicator.GetComponent<IndicatorScript>().Activate(amount);
    }

}
