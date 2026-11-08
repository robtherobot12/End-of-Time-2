using UnityEngine;

public class Repulse : MonoBehaviour
{
    public bool isBlackHole;
    
    public float strength = 20f;
    public float maxRange = 5f;
    public float softening = 0.1f;

    public Rigidbody2D rb;
    public static readonly System.Collections.Generic.List<Repulse> all = new();

    void Awake() { rb = GetComponent<Rigidbody2D>(); }
    void OnEnable()  => all.Add(this);
    void OnDisable() => all.Remove(this);

    void FixedUpdate()
    {
        foreach (var other in all)
        {
            if (other == this) continue;

            if (isBlackHole && !other.isBlackHole) continue;
            if (!isBlackHole && other.isBlackHole) continue;

            Vector2 dir = other.rb.position - rb.position;
            float distSq = dir.sqrMagnitude;
            if (distSq > maxRange * maxRange) continue;

            float force = strength * rb.mass * other.rb.mass / (distSq + softening);
            // negative = repel; make it positive for attraction
            rb.AddForce(-dir.normalized * force);
        }
    }
}
