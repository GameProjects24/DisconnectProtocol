using DisconnectProtocol;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadingPanel;

    [Header("Scene Management")]
    private string gameSceneName;
    public string mainMenuSceneName = "MainMenu";

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		loadingPanel.SetActive(false);
		GameStateController.Instance.ChangeState<GameplayState>();
	}

    private void Start()
    {
        gameSceneName = SceneManager.GetActiveScene().name;
    }

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void OnResume()
	{
		GameStateController.Instance.ChangeState<GameplayState>();
	}

    public void OnRestart()
    {
		loadingPanel.SetActive(true);
		Destroy(transform.root.gameObject);
		GameController.instance.ChangeScene(gameSceneName);
    }

    public void OnMainMenu()
    {
        Debug.Log("Выход в главное меню...");
		loadingPanel.SetActive(true);
		Destroy(transform.root.gameObject);
		GameController.instance.ChangeScene(mainMenuSceneName);
    }
}
