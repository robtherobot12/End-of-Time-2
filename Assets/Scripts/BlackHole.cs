using UnityEngine;
using UnityEngine.EventSystems;

public class BlackHole : MonoBehaviour, IPointerClickHandler
{
    public float sinkRate;
    float health = 10;

    [SerializeField] GameObject linePrefab;

    GameObject wire;

    GameManager gm;

    void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }

    public void SetWire(GameObject _wire)
    {
        wire = _wire;
        wire.GetComponent<Wire>().startPoint = transform;
        wire.GetComponent<Wire>().endPoint = gm.centreClock;
    }

    void Update()
    {
        gm.IncreaseSinkRate(Time.deltaTime * sinkRate);
    }

    public void OnPointerClick(PointerEventData e)
    {
        health -= 1;
        if (health < 0)
        {
            // dead
            Destroy(wire);
            Destroy(this);
        }
    }
}
