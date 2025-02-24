using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadingPanel;

    [Header("Scene Management")]
    private string gameSceneName;
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        gameSceneName = SceneManager.GetActiveScene().name;
    }

    public void OnRestart()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(gameSceneName);
        asyncOperation.completed += (operation) => Debug.Log("Загрузка завершена!");
    }

    // Метод для выхода из игры
    public void OnMainMenu()
    {
        Debug.Log("Выход в главное меню...");

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(mainMenuSceneName);
        asyncOperation.completed += (operation) => Debug.Log("Загрузка завершена!");
    }

    // Загрузка сцены игры асинхронно с отображением панели загрузки
    private System.Collections.IEnumerator LoadGameSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // Пока сцена не будет загружена
        while (!asyncOperation.isDone)
        {
            yield return null; // Подождать следующий кадр
        }
    }
}
