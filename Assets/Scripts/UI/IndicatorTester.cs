using UnityEngine;
using UnityEngine.InputSystem;

public class IndicatorTester : MonoBehaviour
{
    public GameObject indicatorPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject indicator = Instantiate(indicatorPrefab, pos, Quaternion.identity);
            indicator.transform.SetParent(transform);
        }
    }
}
