using System.Collections.Generic;
using UnityEngine;

/// Pixel-art animated wire between two transforms (XY plane).
/// Draws the wire, travelling energy pulses and sparks pixel-by-pixel into a
/// small texture that follows the wire, so everything sits on the PPU grid.
/// Replaces the LineRenderer version: if a LineRenderer is still attached it is disabled.
public class Wire : MonoBehaviour
{
    [Header("Target Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Pixel art")]
    public int pixelsPerUnit = 32;
    [Range(1, 4)] public int thicknessPixels = 1;
    public int sortingOrder = -10;

    [Header("Wave")]
    public float waveFrequency = 5f;    // how many wiggles along the line
    public float waveAmplitude = 0.5f;  // world units
    public float wiggleSpeed = 10f;

    [Header("Wire colour (breathes between A and B)")]
    public Color wireColourA = new Color32(60, 80, 150, 255);
    public Color wireColourB = new Color32(100, 130, 220, 255);
    public float breatheSpeed = 2f;
    [Range(2, 8)] public int colourSteps = 4;   // stepped colours = pixel-art look

    [Header("Pulses (travel from start to end)")]
    public Color pulseColour = new Color32(140, 240, 255, 255);
    public float pulseSpeed = 60f;      // pixels per second
    public float pulseSpacing = 40f;    // pixels between pulses
    public float pulseLength = 8f;      // pixels

    [Header("Sparks")]
    public Color sparkColour = new Color32(255, 255, 220, 255);
    public float crackleRate = 6f;                          // sparks per second jumping off the wire
    public int burstCount = 5;                              // sparks when a pulse reaches the end
    public Vector2 sparkSpeed = new Vector2(15f, 45f);      // pixels per second
    public Vector2 sparkLife = new Vector2(0.2f, 0.45f);    // seconds
    public int maxSparks = 64;

    struct Spark { public Vector2 pos, vel; public float life, maxLife; }

    const int Margin = 12;   // extra pixels around the wire so sparks have room

    readonly List<Spark> sparks = new List<Spark>();
    readonly List<int> dirty = new List<int>();
    SpriteRenderer sr;
    Texture2D tex;
    Color32[] pixels;
    int texW, texH;
    Vector2Int origin;
    int lastPulseIndex = int.MinValue;
    float crackleAccumulator;

    void Start()
    {
        if (TryGetComponent(out LineRenderer lr)) lr.enabled = false;

        var go = new GameObject("WireGraphic");
        go.transform.SetParent(transform, false);
        sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = sortingOrder;
    }

    // LateUpdate so the wire follows upgrades after they've moved this frame
    void LateUpdate()
    {
        if (sr == null) return;
        if (startPoint == null || endPoint == null) { sr.enabled = false; return; }
        sr.enabled = true;

        // Work in pixel space
        Vector2 s = (Vector2)startPoint.position * pixelsPerUnit;
        Vector2 e = (Vector2)endPoint.position * pixelsPerUnit;
        Vector2 dir = e - s;
        float len = dir.magnitude;
        Vector2 normal = len > 0.001f ? new Vector2(dir.y, -dir.x) / len : Vector2.up;

        UpdateSparks(s, e, normal, len);

        ClearDirty();
        EnsureTexture(s, e);
        DrawWire(s, e, normal, len);
        DrawSparks();

        tex.SetPixels32(pixels);
        tex.Apply(false);
    }

    // ---------- Path ----------

    Vector2 PathPx(float t, Vector2 s, Vector2 e, Vector2 normal)
    {
        Vector2 p = Vector2.Lerp(s, e, t);
        float wave = Mathf.Sin(t * waveFrequency + Time.time * wiggleSpeed);
        float fade = Mathf.Sin(t * Mathf.PI);   // keeps the ends attached
        return p + normal * (wave * waveAmplitude * pixelsPerUnit * fade);
    }

    // ---------- Drawing ----------

    void DrawWire(Vector2 s, Vector2 e, Vector2 normal, float len)
    {
        float breathe = Quantize(0.5f + 0.5f * Mathf.Sin(Time.time * breatheSpeed));
        Color baseCol = Color.Lerp(wireColourA, wireColourB, breathe);

        float waveLen = Mathf.Abs(waveAmplitude * pixelsPerUnit * waveFrequency);
        int samples = Mathf.Clamp(Mathf.CeilToInt((len + waveLen) * 2f), 2, 4000);

        Vector2Int prev = Floor(PathPx(0f, s, e, normal));
        for (int i = 1; i < samples; i++)
        {
            float t = i / (float)(samples - 1);
            Vector2Int cur = Floor(PathPx(t, s, e, normal));
            Line(prev, cur, WireColour(t * len, baseCol));
            prev = cur;
        }
    }

    Color32 WireColour(float distPx, Color baseCol)
    {
        if (pulseSpacing <= 0f) return baseCol;
        float local = Mathf.Repeat(distPx - Time.time * pulseSpeed, pulseSpacing);
        if (local < pulseLength)
        {
            // Bright head, dimmer tail
            float k = Quantize(local / pulseLength);
            return Color.Lerp(baseCol, pulseColour, k);
        }
        return baseCol;
    }

    void DrawSparks()
    {
        foreach (var sp in sparks)
        {
            float a = Quantize(sp.life / sp.maxLife);
            if (a <= 0f) continue;

            Color c = sparkColour; c.a *= a;
            Vector2Int p = Floor(sp.pos);
            PlotOne(p.x, p.y, c);

            // 1px trail for fast sparks
            if (sp.vel.sqrMagnitude > 400f)
            {
                Color tc = c; tc.a *= 0.5f;
                Vector2Int tp = Floor(sp.pos - sp.vel.normalized * 1.5f);
                if (tp != p) PlotOne(tp.x, tp.y, tc);
            }
        }
    }

    // ---------- Sparks ----------

    void UpdateSparks(Vector2 s, Vector2 e, Vector2 normal, float len)
    {
        float dt = Time.deltaTime;

        // Burst when a pulse head reaches the end
        if (pulseSpacing > 0f && pulseSpeed > 0f)
        {
            int index = Mathf.FloorToInt((len - Time.time * pulseSpeed - pulseLength) / pulseSpacing);
            if (lastPulseIndex != int.MinValue && index < lastPulseIndex)
                for (int i = 0; i < burstCount; i++)
                    SpawnSpark(e, Random.insideUnitCircle.normalized * Random.Range(sparkSpeed.x, sparkSpeed.y));
            lastPulseIndex = index;
        }

        // Random crackle along the wire
        crackleAccumulator += crackleRate * dt;
        while (crackleAccumulator >= 1f)
        {
            crackleAccumulator -= 1f;
            Vector2 pos = PathPx(Random.value, s, e, normal);
            float side = Random.value < 0.5f ? -1f : 1f;
            Vector2 vel = normal * side * Random.Range(sparkSpeed.x, sparkSpeed.y) * 0.5f
                        + Random.insideUnitCircle * 5f;
            SpawnSpark(pos, vel);
        }

        // Move and age
        for (int i = sparks.Count - 1; i >= 0; i--)
        {
            Spark sp = sparks[i];
            sp.life -= dt;
            if (sp.life <= 0f)
            {
                sparks[i] = sparks[sparks.Count - 1];
                sparks.RemoveAt(sparks.Count - 1);
                continue;
            }
            sp.pos += sp.vel * dt;
            sp.vel *= Mathf.Clamp01(1f - 3f * dt);   // drag
            sparks[i] = sp;
        }
    }

    void SpawnSpark(Vector2 pos, Vector2 vel)
    {
        if (sparks.Count >= maxSparks) return;
        float life = Random.Range(sparkLife.x, sparkLife.y);
        sparks.Add(new Spark { pos = pos, vel = vel, life = life, maxLife = life });
    }

    // ---------- Texture ----------

    void EnsureTexture(Vector2 s, Vector2 e)
    {
        float amp = Mathf.Abs(waveAmplitude) * pixelsPerUnit + thicknessPixels;
        int minX = Mathf.FloorToInt(Mathf.Min(s.x, e.x) - amp) - Margin;
        int minY = Mathf.FloorToInt(Mathf.Min(s.y, e.y) - amp) - Margin;
        int maxX = Mathf.CeilToInt(Mathf.Max(s.x, e.x) + amp) + Margin;
        int maxY = Mathf.CeilToInt(Mathf.Max(s.y, e.y) + amp) + Margin;
        int needW = maxX - minX + 1;
        int needH = maxY - minY + 1;

        // Reallocate only when it no longer fits or is far too big (rounded up to avoid thrashing)
        if (tex == null || needW > texW || needH > texH || needW * 2 < texW || needH * 2 < texH)
            Allocate(RoundUp(needW, 32), RoundUp(needH, 32));

        origin = new Vector2Int(minX, minY);
        sr.transform.position = new Vector3((float)minX / pixelsPerUnit, (float)minY / pixelsPerUnit, transform.position.z);
    }

    void Allocate(int w, int h)
    {
        if (tex != null) { Destroy(sr.sprite); Destroy(tex); }

        texW = w; texH = h;
        tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        pixels = new Color32[w * h];
        dirty.Clear();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), Vector2.zero, pixelsPerUnit);
    }

    void ClearDirty()
    {
        if (pixels == null) return;
        foreach (int i in dirty) pixels[i] = default;
        dirty.Clear();
    }

    void Plot(int x, int y, Color32 c)
    {
        int start = -(thicknessPixels - 1) / 2;
        for (int dy = 0; dy < thicknessPixels; dy++)
            for (int dx = 0; dx < thicknessPixels; dx++)
                PlotOne(x + start + dx, y + start + dy, c);
    }

    void PlotOne(int x, int y, Color32 c)
    {
        if (c.a == 0) return;
        int lx = x - origin.x, ly = y - origin.y;
        if (lx < 0 || ly < 0 || lx >= texW || ly >= texH) return;
        int i = ly * texW + lx;
        if (pixels[i].a == 0) dirty.Add(i);
        pixels[i] = c;
    }

    // Bresenham
    void Line(Vector2Int a, Vector2Int b, Color32 c)
    {
        int dx = Mathf.Abs(b.x - a.x), sx = a.x < b.x ? 1 : -1;
        int dy = -Mathf.Abs(b.y - a.y), sy = a.y < b.y ? 1 : -1;
        int err = dx + dy, x = a.x, y = a.y;
        while (true)
        {
            Plot(x, y, c);
            if (x == b.x && y == b.y) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x += sx; }
            if (e2 <= dx) { err += dx; y += sy; }
        }
    }

    // ---------- Helpers ----------

    float Quantize(float v) => Mathf.Round(Mathf.Clamp01(v) * (colourSteps - 1)) / (colourSteps - 1);
    static Vector2Int Floor(Vector2 v) => new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    static int RoundUp(int v, int m) => (v + m - 1) / m * m;

    void OnDestroy()
    {
        if (sr != null && sr.sprite != null) Destroy(sr.sprite);
        if (tex != null) Destroy(tex);
    }
}