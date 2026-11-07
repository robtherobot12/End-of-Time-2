using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class IndicatorScript : MonoBehaviour
{
    public float initSpeed = 1f;
    public float lerpVal = 1f;
    public float fadeTime = 1f;

    private float speed;
    private TMP_Text text;



    private void Start()
    {
        speed = initSpeed;
        text = GetComponent<TMP_Text>();
        text.CrossFadeAlpha(0f, fadeTime, true);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
        speed = Mathf.Lerp(speed, 0f, Time.deltaTime * lerpVal);
    }
}