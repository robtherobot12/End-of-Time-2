using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public float pulseRate = 1;
    public float pulseAmount;

    GameManager gm;

    void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Tick());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Tick()
    {
        while(true)
        {
            yield return new WaitForSeconds(pulseRate);
            gm.UpdateMoney(pulseAmount);
        }
    }
}
