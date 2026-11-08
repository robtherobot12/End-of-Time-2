using UnityEngine;
using DG.Tweening;

public class Swing : MonoBehaviour
{
    public Vector3 angleA = new Vector3(0, 0, -30);
    public Vector3 angleB = new Vector3(0, 0, 30);
    public float duration = 1.5f;

    void Start()
    {
        transform.localEulerAngles = angleA;
        
        transform.DOLocalRotate(angleB, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
