using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class TooltipController : MonoBehaviour
{
    public UpgradeChannel showToolTipChannel;
    public EventChannel hideTooltipChannel;

    public TMP_Text itemName;
    public TMP_Text description;
    public TMP_Text price;

    RectTransform rect;
    CanvasGroup group;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
        group.blocksRaycasts = false;   // tooltip must never steal the mouse
        Hide();
    }

    void OnEnable()
    {
        showToolTipChannel._event.AddListener(Show);
        hideTooltipChannel._event.AddListener(Hide);
    }

    void OnDisable()
    {
        showToolTipChannel._event.RemoveListener(Show);
        hideTooltipChannel._event.RemoveListener(Hide);
    }

    void Show(ShopItem item)
    {
        itemName.text = item.displayName;
        description.text = item.description;
        price.text = item.price.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        group.alpha = 1f;
    }

    void Hide()
    {
        group.alpha = 0f;
    }

    void Update()
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        bool rightHalf = pos.x > Screen.width * 0.5f;

        rect.pivot = new Vector2(rightHalf ? 1f : 0f, rect.pivot.y);
        transform.position = pos;
    }
}
