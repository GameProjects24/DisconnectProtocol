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

	public event System.Func<IEnumerator> LevelLoaded;

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
		var ds = FindAnyObjectByType<DropSettings>();
		if (ds != null) {
			DropController.Init(ds.incompatible, ds.other);
		}
		StartCoroutine(SceneLoaded(scene.name));
	}

	private IEnumerator SceneLoaded(string scene)
	{
		bool isLoaded = TryLoadPlayerData(scene);
		if (LevelLoaded != null)
		{
			foreach (System.Func<IEnumerator> hand in LevelLoaded.GetInvocationList())
			{
				yield return hand.Invoke();
			}
		}
		if (isLoaded) {
			yield break;
		}
		StartCoroutine(SavePlayerData(scene));
	}

	private IEnumerator SavePlayerData(string scene)
	{
		var pd = new PlayerData();
		pd.lastLevel = scene;
		pd.location = LocationData.FromTransform(_playerTr);
		pd.inventory = _player.inventory.ToInventoryData();
		GameController.instance.pd = pd;
		yield break;
	}

	private bool TryLoadPlayerData(string scene)
	{
		var pd = GameController.instance.pd;
		if (scene != pd.lastLevel) {
			return false;
		}
		pd.location.ToTransform(ref _playerTr);
		_player.inventory.FromInventoryData(pd.inventory);
		return true;
	}
}