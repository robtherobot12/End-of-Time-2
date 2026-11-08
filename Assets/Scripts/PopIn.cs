using UnityEngine;

/// Scales the object from small up to its normal size when it appears, with a little bounce.
/// Drop it on the sprite/image in your prefab. Plays on Instantiate and whenever it's re-enabled (pooling).
public class PopIn : MonoBehaviour
{
    public float duration = 0.3f;
    [Range(0f, 1f)] public float startScale = 0f;
    public bool bounce = true;            // overshoot slightly before settling
    public bool useUnscaledTime = false;

    Vector3 targetScale;
    float t;
    bool initialised;

    void Awake()
    {
        targetScale = transform.localScale;
        initialised = true;
    }

    void OnEnable()
    {
        if (!initialised) return;
        t = 0f;
        transform.localScale = targetScale * startScale;
    }

    void Update()
    {
        if (t >= duration) return;

        t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float p = Mathf.Clamp01(t / duration);
        float eased = bounce ? EaseOutBack(p) : 1f - (1f - p) * (1f - p);
        transform.localScale = targetScale * Mathf.LerpUnclamped(startScale, 1f, eased);
    }

    static float EaseOutBack(float p)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        float q = p - 1f;
        return 1f + c3 * q * q * q + c1 * q * q;
    }
}