using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipController : MonoBehaviour
{
    public UpgradeChannel showToolTipChannel;
    public EventChannel hideTooltipChannel;

    public TMP_Text itemName;
    public TMP_Text description;
    public TMP_Text price;

    RectTransform rect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        showToolTipChannel._event.AddListener((item) => UpdateInfo(item));
        hideTooltipChannel._event.AddListener(() => gameObject.SetActive(false));
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        bool rightHalf = pos.x > Screen.width * 0.5f;

        rect.pivot = new Vector2(rightHalf ? 1f : 0f, rect.pivot.y);
        transform.position = pos;
    }

    void UpdateInfo(ShopItem item)
    {
        itemName.text = item.displayName;
        description.text = item.description;
        price.text = item.price.ToString();
    }
}
