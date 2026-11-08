using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonBounce : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Transform target;      // the ShopSlot root
    [SerializeField] Button button;         // optional: skip bounce when not clickable

    [Header("Hover")]
    public float hoverScale = 1.1f;
    public float hoverDuration = 0.15f;
    public float exitDuration = 20f;

    [Header("Click bounce")]
    public float punchAmount = 0.15f;       // how hard it bounces
    public float punchDuration = 0.3f;
    public int punchVibrato = 6;            // how many wobbles

    Vector3 baseScale;
    bool isHovering;

    void Awake()
    {
        if (target == null) target = transform;
        baseScale = target.localScale;
    }

    void OnDisable()
    {
        target.DOKill();
        target.localScale = baseScale;
    }

    Vector3 RestingScale => isHovering ? baseScale * hoverScale : baseScale;

    public void OnPointerEnter(PointerEventData e)
    {
        isHovering = true;
        target.DOKill();
        target.DOScale(RestingScale, hoverDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData e)
    {
        isHovering = false;
        target.DOKill();
        target.DOScale(RestingScale, exitDuration).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (button != null && !button.interactable) return;

        target.DOKill();
        target.localScale = RestingScale;   // start from a clean size so bounces don't stack
        target.DOPunchScale(Vector3.one * punchAmount, punchDuration, punchVibrato, 0.5f)
              .SetUpdate(true);
    }
}
