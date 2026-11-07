using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text timer;
    [SerializeField] Transform clockParent;
    [SerializeField] Transform lineParent;

    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject blackHolePrefab;

    public Transform centreClock;

    float sinkRate = 1f;
    
    float money;

    void Start()
    {
        StartCoroutine(SpawnBlackHoles());
    }
    
    public void ClickClock() {
        UpdateMoney(1);
    }

    public void UpdateMoney(float change)
    {
        money += change;
        timer.text = Mathf.RoundToInt(money).ToString();
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
