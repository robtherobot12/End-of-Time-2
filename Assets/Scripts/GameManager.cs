using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text timer;

    float sinkRate = 1f;
    
    float money;
    
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
}
