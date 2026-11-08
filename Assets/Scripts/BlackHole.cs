using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using DG.Tweening;

public class BlackHole : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] SpriteRenderer image;
    [SerializeField] Color flashColor;
    [SerializeField] Color colorA, colorB, pulseColor, sparkColor;

    [SerializeField] CinemachineImpulseSource impulseSource;

    public EventChannel bhDamage;
    public EventChannel bhDeath;
    public EventChannel bhSpawn;
    
    //public float sinkRate;
    float sinkRate = 0.03f;
    float health = 1;

    [SerializeField] GameObject linePrefab;

    GameObject wire;

    GameManager gm;

    void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
        bhSpawn.Call();
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
        Wire wireComponent = wire.GetComponent<Wire>();
        wireComponent.startPoint = transform;
        wireComponent.endPoint = gm.centreClock;
        /*wireComponent.wireColourA = colorA;
        wireComponent.wireColourB = colorB;
        wireComponent.pulseColour = pulseColor;
        wireComponent.sparkColour = sparkColor;*/
        wireComponent.wireColourA = pulseColor;
        wireComponent.wireColourB = sparkColor;
        wireComponent.pulseColour = colorA;
        wireComponent.sparkColour = colorB;
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
            bhDeath.Call();
            impulseSource.GenerateImpulse();
            Destroy(wire);
            Destroy(gameObject);
        } else
        {
            bhDamage.Call();
            image.DOKill(true); // stop any tween already running on this image
            image.DOColor(flashColor, 0.05f)
             .SetLoops(2, LoopType.Yoyo)
             .SetEase(Ease.OutQuad);
        }
    }
}
