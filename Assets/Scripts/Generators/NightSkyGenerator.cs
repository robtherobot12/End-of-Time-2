using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// Procedural pixel-art deep-space background.
/// Twinkling stars on several depth layers that shift slightly with the mouse.
/// Everything is snapped to the pixel grid so it stays crisp at your PPU.
/// Drop on an empty GameObject. No sprites needed.
public class NightSkyGenerator : MonoBehaviour
{
    [Header("Pixel grid")]
    public int pixelsPerUnit = 32;

    [Header("Area in world units (0,0 = fit main camera)")]
    public Vector2 size = Vector2.zero;

    [Header("Background")]
    public Color topColour = new Color32(6, 6, 16, 255);
    public Color bottomColour = new Color32(12, 10, 26, 255);
    [Range(1, 16)] public int gradientBands = 4;   // 1 = flat colour

    [Header("Stars")]
    public int starCount = 160;
    [Range(0f, 1f)] public float bigStarChance = 0.05f;
    public Vector2 twinkleSpeedRange = new Vector2(0.5f, 2.5f);
    [Range(2, 8)] public int brightnessSteps = 4;   // stepped twinkle = more "pixel" feel
    public Color starColour = Color.white;

    [Header("Mouse parallax")]
    [Range(1, 6)] public int depthLayers = 3;
    public float maxOffsetPixels = 3f;   // how far the nearest layer moves, in pixels
    public float smoothing = 4f;         // higher = follows the mouse faster

    [Header("Shooting stars")]
    public bool shootingStars = false;
    public Vector2 shootingStarInterval = new Vector2(8f, 20f);
    public float shootingStarSpeed = 12f;           // world units per second

    [Header("Rendering")]
    public int sortingOrder = -100;

    class Star
    {
        public SpriteRenderer sr;
        public Vector2 basePos;
        public float depth;          // 0..1, 1 = nearest
        public float baseAlpha, speed, phase;
    }

    readonly List<Star> stars = new List<Star>();
    SpriteRenderer shooter;
    Vector2 shooterPos;
    bool shooterActive;
    float nextShooter;
    Vector2 mouse;   // smoothed, -1..1 from screen centre

    void Start()
    {
        var cam = Camera.main;
        if (size == Vector2.zero && cam != null && cam.orthographic)
        {
            float h = cam.orthographicSize * 2f;
            float aspect = Mathf.Max((float)Screen.width / Screen.height, 16f / 9f);
            size = new Vector2(h * aspect, h);

        }

        // Pad the area so the parallax never reveals empty edges
        size += Vector2.one * (maxOffsetPixels * 2f + 2f) / pixelsPerUnit;

        BuildBackground();
        BuildStars();
        BuildShooter();
        nextShooter = Time.time + Random.Range(shootingStarInterval.x, shootingStarInterval.y);
    }

    void Update()
    {
        float t = Time.time;

        // Mouse position normalised to -1..1, smoothed
        Vector2 target = GetMouseNormalised();
        mouse = Vector2.Lerp(mouse, target, 1f - Mathf.Exp(-smoothing * Time.deltaTime));

        // Opposite to the mouse, so the scene feels like it tilts toward the cursor
        Vector2 offset = -mouse * maxOffsetPixels / pixelsPerUnit;

        foreach (var s in stars)
        {
            s.sr.transform.localPosition = Snap(s.basePos + offset * s.depth);

            float b = 0.5f + 0.5f * Mathf.Sin(t * s.speed + s.phase);
            b = Mathf.Round(b * (brightnessSteps - 1)) / (brightnessSteps - 1);
            b = Mathf.Lerp(0.3f, 1f, b);
            Color c = starColour;
            c.a = s.baseAlpha * b;
            s.sr.color = c;
        }

        if (shootingStars) UpdateShooter(t, offset);
    }

    Vector2 GetMouseNormalised()
    {
        Vector2 m;
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null) return Vector2.zero;
        m = Mouse.current.position.ReadValue();
#else
        m = Input.mousePosition;
#endif
        Vector2 n = new Vector2(m.x / Screen.width, m.y / Screen.height) * 2f - Vector2.one;
        return Vector2.ClampMagnitude(n, 1.5f); // stops it jumping when the cursor leaves the window
    }

    void UpdateShooter(float t, Vector2 offset)
    {
        if (!shooterActive && t >= nextShooter)
        {
            shooterActive = true;
            shooterPos = new Vector2(Random.Range(-size.x / 2f, size.x * 0.2f), size.y / 2f);
            shooter.enabled = true;
        }

        if (shooterActive)
        {
            shooterPos += new Vector2(1f, -1f) * shootingStarSpeed * Time.deltaTime;
            shooter.transform.localPosition = Snap(shooterPos + offset);
            if (shooterPos.x > size.x / 2f || shooterPos.y < -size.y / 2f)
            {
                shooterActive = false;
                shooter.enabled = false;
                nextShooter = t + Random.Range(shootingStarInterval.x, shootingStarInterval.y);
            }
        }
    }

    // ---------- Building ----------

    void BuildBackground()
    {
        var tex = new Texture2D(1, gradientBands, TextureFormat.RGBA32, false)
        { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };

        for (int i = 0; i < gradientBands; i++)
        {
            float k = gradientBands == 1 ? 0f : i / (float)(gradientBands - 1);
            tex.SetPixel(0, i, Color.Lerp(bottomColour, topColour, k));
        }
        tex.Apply();

        var sprite = Sprite.Create(tex, new Rect(0, 0, 1, gradientBands), Vector2.zero, 1);
        var sr = CreateChild("Background", sprite, sortingOrder);
        sr.transform.localPosition = Snap(-size / 2f);
        sr.transform.localScale = new Vector3(size.x, size.y / gradientBands, 1f);
    }

    void BuildStars()
    {
        // Digits are alpha (1-9), '.' is transparent
        Sprite small = MakeSprite("9");
        Sprite medium = MakeSprite(".4.", "494", ".4.");
        Sprite big = MakeSprite("..3..", "..6..", "36963", "..6..", "..3..");

        for (int i = 0; i < starCount; i++)
        {
            int layer = Random.Range(0, depthLayers);           // 0 = farthest
            float depth = (layer + 1f) / depthLayers;

            // Bigger, brighter stars only on nearer layers
            float r = Random.value;
            Sprite s = small;
            if (layer == depthLayers - 1 && r < bigStarChance * depthLayers) s = big;
            else if (layer >= depthLayers / 2 && r < bigStarChance * 4f) s = medium;

            var sr = CreateChild("Star", s, sortingOrder + 1 + layer);
            var star = new Star
            {
                sr = sr,
                basePos = new Vector2(Random.Range(-size.x / 2f, size.x / 2f),
                                      Random.Range(-size.y / 2f, size.y / 2f)),
                depth = depth,
                baseAlpha = Mathf.Lerp(0.35f, 1f, depth) * Random.Range(0.7f, 1f),
                speed = Random.Range(twinkleSpeedRange.x, twinkleSpeedRange.y),
                phase = Random.Range(0f, Mathf.PI * 2f)
            };
            sr.transform.localPosition = Snap(star.basePos);
            stars.Add(star);
        }
    }

    void BuildShooter()
    {
        Sprite trail = MakeSprite(
            "1.....",
            ".2....",
            "..4...",
            "...6..",
            "....8.",
            ".....9");
        shooter = CreateChild("ShootingStar", trail, sortingOrder + 1 + depthLayers);
        shooter.color = starColour;
        shooter.enabled = false;
    }

    // ---------- Helpers ----------

    SpriteRenderer CreateChild(string name, Sprite sprite, int order)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = order;
        return sr;
    }

    Sprite MakeSprite(params string[] rows)
    {
        int h = rows.Length, w = rows[0].Length;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                char c = rows[h - 1 - y][x];
                float a = char.IsDigit(c) ? (c - '0') / 9f : 0f;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();

        // Bottom-left pivot so snapped positions land exactly on pixel boundaries
        return Sprite.Create(tex, new Rect(0, 0, w, h), Vector2.zero, pixelsPerUnit);
    }

    float Snap(float v) => Mathf.Round(v * pixelsPerUnit) / pixelsPerUnit;
    Vector3 Snap(Vector2 v) => new Vector3(Snap(v.x), Snap(v.y), 0f);
}