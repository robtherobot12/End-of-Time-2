using UnityEngine;

[CreateAssetMenu(fileName = "Clock Data", menuName = "ScriptableObjects/ClockData")]
public class ClockData : ScriptableObject
{
    public GameObject prefab;
    public float price;
}
