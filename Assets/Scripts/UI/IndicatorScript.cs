using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class IndicatorScript : MonoBehaviour
{
    public Vector3 targetOffset;
    public float lerpVal = 0.1f;
    public float fadeTime = 1f;

    private TMP_Text text;
    private Vector3 targetPos;
    private bool canIndicate = false;


    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Activate()
    {
        canIndicate = true;
        targetPos = transform.position + targetOffset;
        text.CrossFadeAlpha(0f, fadeTime, true);
        Destroy(gameObject, fadeTime);
    }

    private void Update()
    {
        if(canIndicate)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpVal);
        }
    }
}