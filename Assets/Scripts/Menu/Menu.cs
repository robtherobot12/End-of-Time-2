using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] RectTransform title;
    [SerializeField] private float loadDelay = 0.25f;  

    void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(StartGame);


        SceneTransition.Instance.Open(SceneTransition.ScreenPos(title));
    }

    void StartGame()
    {
        button.interactable = false;
        SceneTransition.Instance.LoadScene("Game", SceneTransition.ScreenPos(title));   
    }

}
