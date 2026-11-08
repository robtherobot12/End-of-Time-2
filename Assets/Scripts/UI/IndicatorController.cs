using UnityEngine;
using UnityEngine.InputSystem;

public class IndicatorController : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public EventChannel indicatorEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indicatorEvent?._event.AddListener(() => CreateIndicator());
    }

    public void CreateIndicator()
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        GameObject indicator = Instantiate(indicatorPrefab, pos, Quaternion.identity);
        indicator.GetComponent<IndicatorScript>().Activate(1);
        indicator.transform.SetParent(transform);
    }
}
