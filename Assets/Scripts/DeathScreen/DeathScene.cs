using UnityEngine;
using UnityEngine.UI;

public class DeathScene : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] RectTransform title; 

    void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(BackToMenu);

        SceneTransition.Instance.Open(SceneTransition.ScreenPos(title));
    }

    void BackToMenu()
    {
        button.interactable = false;
        SceneTransition.Instance.LoadScene("StartMenu", SceneTransition.ScreenPos(title));
    }
}
