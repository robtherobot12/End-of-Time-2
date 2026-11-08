using TMPro;
using UnityEngine;

/// Animated title for TextMeshPro (UI or world space).
/// Letters drop in one by one, then bob in a wave, sway slightly, cycle colours,
/// and get a white shimmer sweeping across every few seconds. The whole title also floats gently.
/// Uses unscaled time, so it keeps animating if the game is paused.
[RequireComponent(typeof(TMP_Text))]
public class TitleAnimation : MonoBehaviour
{
    [Header("Intro (letters drop in)")]
    public bool dropIn = true;
    public float dropHeight = 60f;
    public float dropDuration = 0.5f;
    public float dropStagger = 0.06f;     // delay between letters

    [Header("Letter wave")]
    public float waveHeight = 6f;
    public float waveSpeed = 3f;
    public float waveSpacing = 0.5f;      // phase offset between letters

    [Header("Letter sway")]
    public float swayDegrees = 3f;
    public float swaySpeed = 2f;

    [Header("Whole title float")]
    public Vector2 floatAmount = new Vector2(4f, 6f);
    public Vector2 floatSpeed = new Vector2(0.7f, 1.1f);

    [Header("Colour")]
    public bool useGradient = true;
    public Gradient gradient = DefaultGradient();
    public float colourSpeed = 0.12f;     // how fast colours cycle
    public float colourSpacing = 0.05f;   // colour offset between letters
    [Range(0f, 1f)] public float bottomShade = 0.75f;  // darker bottom half of each letter

    [Header("Shimmer")]
    public bool shimmer = true;
    public Color shimmerColour = Color.white;
    public float shimmerInterval = 3.5f;  // seconds between sweeps
    public float shimmerSpeed = 25f;      // letters per second
    public float shimmerWidth = 2f;       // in letters

    [Header("Pixel snapping (0 = off)")]
    [Tooltip("Snap movement to steps of this size. For world-space text at 32 PPU use 0.03125.")]
    public float pixelSize = 0f;

    TMP_Text tmp;
    Vector3 basePos;
    float startTime;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        basePos = transform.localPosition;
    }

    void OnEnable() => PlayIntro();

    /// Restart the drop-in intro.
    public void PlayIntro() => startTime = Time.unscaledTime;

    void Update()
    {
        float t = Time.unscaledTime;
        float since = t - startTime;

        // Whole title float
        Vector2 f = new Vector2(Mathf.Sin(t * floatSpeed.x) * floatAmount.x,
                                Mathf.Sin(t * floatSpeed.y) * floatAmount.y);
        transform.localPosition = basePos + (Vector3)Snap(f);

        tmp.ForceMeshUpdate();
        TMP_TextInfo info = tmp.textInfo;
        int count = info.characterCount;
        float shimmerPos = shimmer ? ShimmerPosition(t, count) : float.MinValue;

        for (int i = 0; i < count; i++)
        {
            TMP_CharacterInfo ch = info.characterInfo[i];
            if (!ch.isVisible) continue;

            int mi = ch.materialReferenceIndex;
            int vi = ch.vertexIndex;
            Vector3[] verts = info.meshInfo[mi].vertices;
            Color32[] cols = info.meshInfo[mi].colors32;

            // Motion
            float y = Mathf.Sin(t * waveSpeed + i * waveSpacing) * waveHeight;
            float angle = Mathf.Sin(t * swaySpeed + i * 0.9f) * swayDegrees;
            float visible = 1f;

            if (dropIn)
            {
                float p = Mathf.Clamp01((since - i * dropStagger) / dropDuration);
                y += (1f - EaseOutBack(p)) * dropHeight;
                if (p <= 0f) visible = 0f;
            }

            Vector3 offset = Snap(new Vector2(0f, y));
            Vector3 centre = (verts[vi] + verts[vi + 2]) * 0.5f;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            for (int j = 0; j < 4; j++)
                verts[vi + j] = rot * (verts[vi + j] - centre) + centre + offset;

            // Colour
            Color top = useGradient
                ? gradient.Evaluate(Mathf.Repeat(t * colourSpeed + i * colourSpacing, 1f))
                : (Color)cols[vi + 1];

            if (shimmer)
            {
                float d = Mathf.Abs(i - shimmerPos);
                if (d < shimmerWidth) top = Color.Lerp(top, shimmerColour, 1f - d / shimmerWidth);
            }

            Color bottom = new Color(top.r * bottomShade, top.g * bottomShade, top.b * bottomShade);

            // Vertex order: 0 bottom-left, 1 top-left, 2 top-right, 3 bottom-right
            for (int j = 0; j < 4; j++)
            {
                Color c = (j == 1 || j == 2) ? top : bottom;
                c.a = cols[vi + j].a / 255f * visible;
                cols[vi + j] = c;
            }
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }

    float ShimmerPosition(float t, int count)
    {
        float phase = Mathf.Repeat(t, shimmerInterval);
        return phase * shimmerSpeed - shimmerWidth;   // sweeps left to right, then waits off the end
    }

    Vector2 Snap(Vector2 v)
    {
        if (pixelSize <= 0f) return v;
        return new Vector2(Mathf.Round(v.x / pixelSize) * pixelSize, Mathf.Round(v.y / pixelSize) * pixelSize);
    }

    static float EaseOutBack(float p)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        float q = p - 1f;
        return 1f + c3 * q * q * q + c1 * q * q;
    }

    static Gradient DefaultGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(new Color32(170, 90, 255, 255), 0f),     // purple
                new GradientColorKey(new Color32(90, 220, 255, 255), 0.35f),  // cyan
                new GradientColorKey(new Color32(255, 140, 230, 255), 0.7f),  // pink
                new GradientColorKey(new Color32(170, 90, 255, 255), 1f),     // back to purple so it loops
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        return g;
    }
}