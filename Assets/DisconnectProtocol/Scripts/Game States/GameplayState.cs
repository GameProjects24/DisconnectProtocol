using UnityEngine;
using DisconnectProtocol;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameplayState : GameState
{
    [Header("State References")]

    public GameObject uiPanel;
    public PlayerControls controls;
    public string inputMapName = "Gameplay";
	private PlayerController _player;
	private Transform _playerTr;

	public event System.Func<IEnumerator> LevelLoaded;

    public override void OnEnter()
    {
        base.OnEnter();
        Time.timeScale = 1f;

        if (uiPanel != null) uiPanel.SetActive(true);
        if (controls != null) controls.SetInputMapState(inputMapName, true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnExit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (uiPanel != null) uiPanel.SetActive(false);
        if (controls != null) controls.SetInputMapState(inputMapName, false);

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
		var dmg = _player.GetComponent<Damageable>();
		if (dmg) {
			dmg.Heal(dmg.maxHp);
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
		// FIX ME
		GameController.instance.SavePlayerData();
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
