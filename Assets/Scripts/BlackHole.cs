using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class BlackHole : MonoBehaviour, IPointerClickHandler
{
    public float sinkRate;
    float health = 1;

    [SerializeField] GameObject linePrefab;

    GameObject wire;

    GameManager gm;

    void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }

    void SetHealth(float newHealth)
    {
        health = newHealth;
        float scale = Mathf.Lerp(0.5f, 1f, health);
        transform.localScale = new Vector3(scale, scale, 1);
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
        SetHealth(health - 0.2f);
        if (health <= 0)
        {
            // dead
            Destroy(wire);
            Destroy(gameObject);
        }
    }
}
