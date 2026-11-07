using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Wire : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Target Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Wave Settings")]
    public int resolution = 30;       // How many segments the line has (higher = smoother)
    public float waveFrequency = 5f;  // How many waves/wiggles along the line
    public float waveAmplitude = 0.5f; // How tall the wiggles are
    public float wiggleSpeed = 10f;   // How fast the wave moves

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        GenerateWave();
    }

    void GenerateWave()
    {
        // 1. Set the number of points on the line renderer
        lineRenderer.positionCount = resolution;

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        // 2. Find the direction and perpendicular vectors for the offset
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;
        Vector3 normal = Vector3.Cross(direction, Vector3.forward).normalized; 
        // If working on XZ plane instead of XY, change Vector3.forward to Vector3.up

        // 3. Calculate each point along the line
        for (int i = 0; i < resolution; i++)
        {
            // Normalize progress between 0 and 1
            float t = (float)i / (resolution - 1);

            // Straight line position
            Vector3 straightLinePos = Vector3.Lerp(startPos, endPos, t);

            // Calculate sine wave animation offset
            // Modifying 't * waveFrequency' creates the wave shape along the distance
            // Adding 'Time.time * wiggleSpeed' makes it animate/crawl
            float waveFactor = Mathf.Sin(t * waveFrequency + Time.time * wiggleSpeed);
            
            // Optional: Fade out wave at the start and end points so they stay attached
            float fadeFactor = Mathf.Sin(t * Mathf.PI); 

            Vector3 offset = normal * waveFactor * waveAmplitude * fadeFactor;

            // Final position is straight position + the perpendicular wiggle
            lineRenderer.SetPosition(i, straightLinePos + offset);
        }
    }
}
