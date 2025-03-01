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
			StartCoroutine(SavePlayerData(scene.name));
		}
	}

	private IEnumerator SavePlayerData(string scene)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		var pd = new PlayerData();
		pd.lastLevel = scene;
		pd.location = LocationData.FromTransform(_playerTr);
		pd.inventory = _player.weaponController.GetInventory();
		GameController.instance.pd = pd;
	}

	private void LoadPlayerData(string scene)
	{
		var pd = GameController.instance.pd;
		pd.location.ToTransform(ref _playerTr);
		_player.weaponController.SetInventory(pd.inventory);
	}
}