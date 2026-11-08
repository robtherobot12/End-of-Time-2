using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] FloatChannel indicator;
    [SerializeField] float addAmount = 10;
    bool isDead;

    public Transform centreClock;

    public bool endGame = false;

    float sinkRate = 0f;
    
    float money = 10f;

    IEnumerator Start()
    { 
        yield return null;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.Open(SceneTransition.ScreenPos(centreClock)); 
    }

    void OnEnable()
    {
        upgradeChannel._event.AddListener(HandleBuy);
        halftimeChannel._event.AddListener(OnHalfTime);
    }

    void OnDisable()
    {
        upgradeChannel._event.RemoveListener(HandleBuy);
        halftimeChannel._event.RemoveListener(OnHalfTime);
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
        UpdateMoney(addAmount);
        indicator.Call(addAmount);
    }

    private void Dead()
    {
        if (isDead) return;     
            isDead = true;
        SceneTransition.Instance.LoadScene("DeathScreen", SceneTransition.ScreenPos(centreClock));

    }

    public void UpdateMoney(float change)
    {
        money += change;
        timer.text = FormatTime(money);
        damageMultiplier.text = $"{sinkRate:0.#} DPS";   

        if (money < 0)
        {
            Dead();
        }
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
        clock.transform.localPosition = GetRandomPosition();
        //clock.transform.GetComponent<ClockMovement>().SetOffset(GetRandomPosition());

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
        if (Random.Range(0, 1f) >= 0.5f)
        {
            return new Vector3(Random.Range(-6, -3), Random.Range(-2, 2), 0);
        }
        else
        {
            return new Vector3(Random.Range(3, 6), Random.Range(-2, 2), 0);
        }
    }

    IEnumerator SpawnBlackHoles()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            BlackHole blackHole = Instantiate(blackHolePrefab, clockParent).GetComponent<BlackHole>();
            blackHole.transform.localPosition = GetRandomPosition();
            //blackHole.transform.GetComponent<BlackHoleMovement>().SetOffset(GetRandomPosition());
            GameObject wire = Instantiate(linePrefab, lineParent);
            blackHole.SetWire(wire);
        }
    }

    private void OnHalfTime()
    {
        StartCoroutine(SpawnBlackHoles());
    }

    static readonly (string single, string plural, double seconds)[] timeUnits =
    {
        ("second",     "seconds",     1),
        ("minute",     "minutes",     60),
        ("hour",       "hours",       3600),
        ("day",        "days",        86400),
        ("week",       "weeks",       604800),
        ("month",      "months",      2629746),
        ("year",       "years",       31556952),
        ("decade",     "decades",     315569520),
        ("century",    "centuries",   3155695200),
        ("millennium", "millennia",   31556952000),
        ("million years", "million years", 3.1556952e13),
        ("billion years", "billion years", 3.1556952e16),
        ("universe age",  "universe ages", 4.354859e17),
    };

    string FormatTime(double seconds)
    {
        int i = 0;
        while (i < timeUnits.Length - 1 && seconds >= timeUnits[i + 1].seconds)
            i++;

        double value = seconds / timeUnits[i].seconds;
        string number = value.ToString("0.#");
        string name = number == "1" ? timeUnits[i].single : timeUnits[i].plural;

        return number + " " + name;
    }
}
