using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class IndicatorScript : MonoBehaviour
{
    public Vector3 targetOffset;
    public float lerpVal = 0.1f;
    public float fadeTime = 1f;
    public TMP_Text text;

    private Vector3 targetPos;
    private bool canIndicate = false;

    public void Activate(float amount)
    {
        text.text = "+" + GameManager.FormatTime(amount);
        canIndicate = true;
        targetPos = transform.localPosition + targetOffset;
        text.CrossFadeAlpha(0f, fadeTime, true);
        Destroy(gameObject, fadeTime);
    }

    private void Update()
    {
        if(canIndicate)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, lerpVal);
        }
    }
}