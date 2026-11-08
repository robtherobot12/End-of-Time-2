using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] Image overlay;          // full-screen Image using the IrisWipe material
    [SerializeField] float closeTime = 0.6f;
    [SerializeField] float openTime = 0.7f;

    Material mat;
    static readonly int RadiusID = Shader.PropertyToID("_Radius");
    static readonly int CenterID = Shader.PropertyToID("_Center");

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            DOTween.useSmoothDeltaTime = true;
            DOTween.maxSmoothUnscaledTime = 0.05f;

            mat = new Material(overlay.material);
            overlay.material = mat;
            SetRadius(0f);
            overlay.raycastTarget = true;
    }

    // Radius big enough to uncover the whole screen from this center
    float FullRadius(Vector2 c)
    {
        float dx = Mathf.Max(c.x, Screen.width - c.x);
        float dy = Mathf.Max(c.y, Screen.height - c.y);
        return Mathf.Sqrt(dx * dx + dy * dy) + 50f;
    }

    void SetCenter(Vector2 c) => mat.SetVector(CenterID, c);
    void SetRadius(float r)   => mat.SetFloat(RadiusID, r);
    float GetRadius()         => mat.GetFloat(RadiusID);

    public void LoadScene(string scene, Vector2 closeCenter)
    {
        StartCoroutine(Run(scene, closeCenter));
    }

    IEnumerator Run(string scene, Vector2 c)
    {
        overlay.raycastTarget = true;   // block clicks during the transition
        SetCenter(c);
        SetRadius(FullRadius(c));

        yield return DOTween.To(GetRadius, SetRadius, 0f, closeTime)
                            .SetEase(Ease.InCubic).SetUpdate(true).WaitForCompletion();

        yield return SceneManager.LoadSceneAsync(scene);
        yield return null;   // give the new scene one frame to run Start()
    }

    // Called by the new scene once it knows where its focus point is
    public void Open(Vector2 openCenter)
    {
        StartCoroutine(OpenWhenReady(openCenter));
    }

    IEnumerator OpenWhenReady(Vector2 c)
    {
        float start = Time.realtimeSinceStartup;
        int smoothFrames = 0;

    
        while ((smoothFrames < 5 || Time.realtimeSinceStartup - start < 0.2f)
            && Time.realtimeSinceStartup - start < 2f)
        {
            smoothFrames = Time.unscaledDeltaTime < 1f / 30f ? smoothFrames + 1 : 0;
            yield return null;
        }

        SetCenter(c);
        DOTween.To(GetRadius, SetRadius, FullRadius(c), openTime)
            .SetEase(Ease.OutCubic).SetUpdate(true)
            .OnComplete(() => overlay.raycastTarget = false);
    }

    // Screen-position helpers
    public static Vector2 ScreenPos(RectTransform ui) =>
        RectTransformUtility.WorldToScreenPoint(Camera.main, ui.position);
    // overlay-canvas UI

    public static Vector2 ScreenPos(Transform world) =>
        Camera.main.WorldToScreenPoint(world.position);                  // sprites in the game world
}
