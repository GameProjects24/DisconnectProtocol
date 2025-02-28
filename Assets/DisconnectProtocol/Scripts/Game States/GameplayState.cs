using UnityEngine;
using DisconnectProtocol;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayState : GameState
{
    [Header("State References")]

    public GameObject uiPanel;
    public InputActionAsset inputActions;
    public InputActionMap inputMap;
	private PlayerController _player;
	private Transform _playerTr;

    private void Awake()
    {
        inputMap = inputActions.FindActionMap("Gameplay");
        //uiActionMap = inputActions.FindActionMap("UI");
    }

	private void Start()
	{
		if (_player == null) {
			_player = FindAnyObjectByType<PlayerController>();
			_playerTr = _player.transform;
		}
		LoadPlayerData(SceneManager.GetActiveScene().name);
	}

    public override void OnEnter()
    {
        base.OnEnter();
        Time.timeScale = 1f;

        if (uiPanel != null) uiPanel.SetActive(true);
        if (inputMap != null) inputMap.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnExit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (uiPanel != null) uiPanel.SetActive(false);
        if (inputMap != null) inputMap.Disable();

        base.OnExit();
    }

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		LoadPlayerData(scene.name);
	}

	private void SavePlayerData()
	{
		Debug.Log($"{_playerTr.position} | {_playerTr.name}");
		GameController.instance.pd.SetLocation(_playerTr);
		GameController.instance.pd.SetInventoryData(_player.weaponController.GetInventoryData());
	}

	private void LoadPlayerData(string scene)
	{
		GameController.instance.pd.TryLoadLocation(scene, ref _playerTr);
		var inv = GameController.instance.pd.TryGetInventoryData(scene);
		if (inv != null)
		{
			_player.weaponController.SetInventoryData(inv);
		}
	}
}
