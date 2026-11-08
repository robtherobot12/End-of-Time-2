using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScene : MonoBehaviour
{
   [SerializeField] private Button button;

    void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
