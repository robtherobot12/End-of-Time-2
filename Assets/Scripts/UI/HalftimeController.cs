using UnityEngine;

public class HalftimeController : MonoBehaviour
{
    [SerializeField] private EventChannel halftime;
    [SerializeField] private HalftimeAnimation anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        halftime?._event.AddListener(() => anim.Show());
    }
}
