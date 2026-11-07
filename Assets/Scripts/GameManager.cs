using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] UpgradeChannel upgradeChannel;
    [SerializeField] EventChannel halftimeChannel;
    
    [SerializeField] TMP_Text timer;

    [SerializeField] TMP_Text damageMultiplier;
    [SerializeField] Transform clockParent;
    [SerializeField] Transform lineParent;

    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject blackHolePrefab;

    public Transform centreClock;

    public bool endGame = false;

    float sinkRate = 1f;
    
    float money;

    void Start()
    {
        StartCoroutine(SpawnBlackHoles());
    }

    void OnEnable()
    {
        upgradeChannel._event.AddListener(HandleBuy);
    }

    void OnDisable()
    {
        upgradeChannel._event.RemoveListener(HandleBuy);
    }

    void HandleBuy(ShopItem item)
    {
        UpdateMoney(-item.price);
        AddClock(item);
        
        if (item.displayName == "Rolex")
        {
            // start transition
            halftimeChannel._event.Invoke();
        }
    }
    
    public void ClickClock() {
        UpdateMoney(1);
    }

    public void UpdateMoney(float change)
    {
        money += change;
        timer.text = Mathf.RoundToInt(money).ToString();
        damageMultiplier.text = Mathf.RoundToInt(sinkRate).ToString();
    }

    public float GetMoney()
    {
        return money;
    }

    void Update()
    {
        UpdateMoney(-Time.deltaTime * sinkRate);
        
    }

    public void AddClock(ShopItem clockItem)
    {
        GameObject clock = Instantiate(clockItem.prefab, clockParent);
        UpdateMoney(-clockItem.price);

        Wire wire = Instantiate(linePrefab, lineParent).GetComponent<Wire>();
        wire.startPoint = clock.transform;
        wire.endPoint = centreClock;
    }

    public void IncreaseSinkRate(float multiplier)
    {
        sinkRate += multiplier;
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-6, -3), Random.Range(-2, 2), 0);
    }

    IEnumerator SpawnBlackHoles()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            BlackHole blackHole = Instantiate(blackHolePrefab, clockParent).GetComponent<BlackHole>();
            blackHole.transform.GetComponent<ClockMovement>().SetOffset(GetRandomPosition());
            GameObject wire = Instantiate(linePrefab, lineParent);
            blackHole.SetWire(wire);
        }
    }
}
