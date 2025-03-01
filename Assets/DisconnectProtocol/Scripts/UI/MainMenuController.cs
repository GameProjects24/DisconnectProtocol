using UnityEngine;
using UnityEngine.SceneManagement;
using DisconnectProtocol;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject loadingPanel;

    [Header("Scene Management")]
    public string gameSceneName = "Map";

    public void OnStartGame()
    {
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(true);

        GameController.instance.ChangeScene(gameSceneName);
    }

    // Метод для выхода из игры
    public void OnExitGame()
    {
        Debug.Log("Выход из игры...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // Остановит игру в редакторе
#else
        Application.Quit();  // Для сборки
#endif
    }
}
