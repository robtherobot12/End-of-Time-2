using UnityEngine;

public class ImgMover : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] float radius = 0.4f;        // max distance from center
    [SerializeField] float speed = 0.6f;         // how fast it wanders
    [SerializeField] float speedVariance = 0.3f; // randomises speed per instance

    [Header("Extra organic feel")]
    [SerializeField] bool addSecondLayer = true; // faster, smaller jitter on top
    [SerializeField] float detailAmount = 0.15f; // fraction of radius
    [SerializeField] float detailSpeedMult = 2.7f;

    Vector3 center;
    Vector2 seedA, seedB;
    float speedX, speedY;


    bool isWobbling = true;


    void Awake()
    {
        // Random seeds so every object wanders differently
        seedA = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));
        seedB = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));

        // Slightly different speed per axis so the path doesn't look diagonal/locked
        speedX = speed * UnityEngine.Random.Range(1f - speedVariance, 1f + speedVariance);
        speedY = speed * UnityEngine.Random.Range(1f - speedVariance, 1f + speedVariance);
    }

    void Update()
    {
        if (isWobbling)
        {
            float t = Time.time;

            // PerlinNoise returns 0..1, remap to -1..1
            float x = Mathf.PerlinNoise(seedA.x + t * speedX, seedA.y) * 2f - 1f;
            float y = Mathf.PerlinNoise(seedB.x, seedB.y + t * speedY) * 2f - 1f;

            Vector2 offset = new Vector2(x, y);

            if (addSecondLayer)
            {
                float dx = Mathf.PerlinNoise(seedB.x + t * speedX * detailSpeedMult, seedA.y + 50f) * 2f - 1f;
                float dy = Mathf.PerlinNoise(seedA.x + 50f, seedB.y + t * speedY * detailSpeedMult) * 2f - 1f;
                offset += new Vector2(dx, dy) * detailAmount;
            }

            // Keep it inside a circle rather than a square
            offset = Vector2.ClampMagnitude(offset, 1f);

            transform.localPosition = center + (Vector3)(offset * radius);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localPosition = center;
        //transform.localPosition = new Vector3(-5,0,0);
        //floatCoroutine = StartCoroutine(Float());
        isWobbling = true;
    }

    public void SetOffset(Vector3 _offset)
    {
        center = _offset;
    }
}
