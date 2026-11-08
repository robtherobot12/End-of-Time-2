using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonBounce : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Scales")]
    public float hoverScale = 1.1f;
    public float pressScale = 0.92f;

    [Header("Timing")]
    public float hoverDuration = 0.2f;
    public float pressDuration = 0.1f;

    Vector3 baseScale;
    bool isHovering;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void OnDisable()
    {
        transform.DOKill();
        transform.localScale = baseScale;   // reset if the button is hidden mid-tween
    }

    public void OnPointerEnter(PointerEventData e)
    {
        isHovering = true;
        transform.DOKill();
        transform.DOScale(baseScale * hoverScale, hoverDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData e)
    {
        isHovering = false;
        transform.DOKill();
        transform.DOScale(baseScale, hoverDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    public void OnPointerDown(PointerEventData e)
    {
        transform.DOKill();
        transform.DOScale(baseScale * pressScale, pressDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData e)
    {
        transform.DOKill();
        Vector3 target = isHovering ? baseScale * hoverScale : baseScale;
        transform.DOScale(target, 0.25f)
            .SetEase(Ease.OutElastic)   // springy release
            .SetUpdate(true);
    }
}
