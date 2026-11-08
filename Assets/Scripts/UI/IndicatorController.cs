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
        GameObject indicator = Instantiate(indicatorPrefab, pos, Quaternion.identity, transform);
        indicator.GetComponent<IndicatorScript>().Activate(amount);
    }
}
