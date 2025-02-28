using UnityEngine;
using DisconnectProtocol;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

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
		if (_player == null) {
			_player = FindAnyObjectByType<PlayerController>();
			_playerTr = _player.transform;
		}
		if (scene.name == GameController.instance.pd.lastLevel) {
			LoadPlayerData(scene.name);
		} else {
			StartCoroutine(SavePlayerData());
			GameController.instance.pd.lastLevel = scene.name;
		}
	}

	private IEnumerator SavePlayerData()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
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