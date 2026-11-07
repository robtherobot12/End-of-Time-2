using UnityEngine;

public class GameManager : MonoBehaviour
{
    public double money;
    
    public void ClickClock() {
        money += 1;
        Debug.Log(money);
    }
}
