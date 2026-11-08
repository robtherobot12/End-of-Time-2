using System.Collections;
using UnityEngine;

/// Timed popup screen: fades in with a bouncing text, waits, then bounces and fades out.
/// Clicking anywhere while it's showing makes it leave early.
/// Put this on your panel (the dimmed background) and assign the text's RectTransform.
///
/// Usage: popup.Show();   or   popup.Show(6f);
[RequireComponent(typeof(CanvasGroup))]
public class HalftimeAnimation : MonoBehaviour
{
    [Header("References")]
    public RectTransform text;            // the text to bounce

    [Header("Timing")]
    public float duration = 7f;           // seconds fully visible
    public float fadeInTime = 0.35f;
    public float fadeOutTime = 0.35f;

    [Header("Bounce")]
    public float bounceInTime = 0.5f;
    public float bounceOutTime = 0.3f;
    [Range(0f, 1f)] public float startScale = 0.3f;

    [Header("Options")]
    public bool showOnStart = false;
    public float clickDelay = 1f;

    CanvasGroup group;
    Coroutine routine;
    bool skipRequested;
    float shownTime;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        SetHidden();
    }

    void Start()
    {
        if (showOnStart) Show();
    }

    public void Show() => Show(duration);

    public void Show(float seconds)
    {
        if (routine != null) StopCoroutine(routine);
        shownTime = Time.unscaledTime;     // so the click that opened it doesn't close it
        routine = StartCoroutine(Run(seconds));
    }

    void Update()
    {
        // Any click while visible skips straight to the exit animation
        if (routine != null && Time.unscaledTime - shownTime >= clickDelay && PressedThisFrame())
            skipRequested = true;
    }

    IEnumerator Run(float seconds)
    {
        skipRequested = false;
        group.blocksRaycasts = true;      // stops clicks reaching the game behind

        // In: fade + bouncy scale-up (a click here skips to the exit)
        float inTime = Mathf.Max(fadeInTime, bounceInTime);
        float t = 0f;
        for (; t < inTime && !skipRequested; t += Time.unscaledDeltaTime)
        {
            group.alpha = Mathf.Clamp01(t / fadeInTime);
            SetTextScale(Mathf.LerpUnclamped(startScale, 1f, EaseOutBack(Mathf.Clamp01(t / bounceInTime))));
            yield return null;
        }

        // Hold
        for (float h = 0f; h < seconds && !skipRequested; h += Time.unscaledDeltaTime)
            yield return null;

        // Out: pull back, shrink, fade (starts from wherever the fade-in got to)
        float startAlpha = group.alpha;
        float startTextScale = text != null ? text.localScale.x : 1f;
        float outTime = Mathf.Max(fadeOutTime, bounceOutTime);
        for (t = 0f; t < outTime; t += Time.unscaledDeltaTime)
        {
            group.alpha = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(t / fadeOutTime));
            SetTextScale(Mathf.LerpUnclamped(startTextScale, 0f, EaseInBack(Mathf.Clamp01(t / bounceOutTime))));
            yield return null;
        }

        SetHidden();
        routine = null;
    }

    bool PressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null && mouse.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    void SetHidden()
    {
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
        SetTextScale(startScale);
    }

    void SetTextScale(float s)
    {
        if (text != null) text.localScale = new Vector3(s, s, 1f);
    }

    // Overshoots past 1, then settles
    static float EaseOutBack(float p)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        float q = p - 1f;
        return 1f + c3 * q * q * q + c1 * q * q;
    }

    // Pulls back slightly before going
    static float EaseInBack(float p)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return c3 * p * p * p - c1 * p * p;
    }
}