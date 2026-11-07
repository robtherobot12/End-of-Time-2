using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private float zDistance = 1.0f;
    [SerializeField] private float parallaxMultiplier = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var v3 = Input.mousePosition;
        v3.z = 10.0;
        v3 = Camera.main.ScreenToWorldPoint(v3);

        foreach (Transform bg in gameObject.GetChildren())
        {
            
        }
    }
}
