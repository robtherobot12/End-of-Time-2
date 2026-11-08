using UnityEngine;

public class FixedRepulse : MonoBehaviour
{
    public bool ignoreBlackHole;
    public float strength = 20f;
    public float maxRange = 5f;
    public float softening = 0.1f;

    //static readonly System.Collections.Generic.List<Repulse> all = new();

    void FixedUpdate()
    {
        foreach (var other in Repulse.all)
        {
            if (ignoreBlackHole && other.isBlackHole) continue;
            
            Vector2 dir = other.rb.position - new Vector2(transform.localPosition.x, transform.localPosition.y);
            float distSq = dir.sqrMagnitude;
            if (distSq > maxRange * maxRange) continue;

            float force = strength * other.rb.mass / (distSq + softening);
            // negative = repel; make it positive for attraction
            other.rb.AddForce(-dir.normalized * force);
        }
    }
}
