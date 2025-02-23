using UnityEngine;
using UnityEngine.SceneManagement;

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

        StartCoroutine(LoadGameSceneAsync());
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

    // Загрузка сцены игры асинхронно с отображением панели загрузки
    private System.Collections.IEnumerator LoadGameSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(gameSceneName);

        // Пока сцена не будет загружена
        while (!asyncOperation.isDone)
        {
            yield return null; // Подождать следующий кадр
        }
    }
}
