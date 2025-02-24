using UnityEngine;
using DisconnectProtocol;
using System.Collections.Generic;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    private Dictionary<System.Type, GameState> states = new Dictionary<System.Type, GameState>();

    private GameState currentState;

    public Damageable damageble;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            GameState[] foundStates = GetComponentsInChildren<GameState>(true);
            foreach (GameState state in foundStates)
            {
                states[state.GetType()] = state;
                state.gameObject.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState<GameplayState>();
    }

    private void OnEnable()
    {
        if (damageble != null)
        {
            damageble.OnDie += OnPlayerDie;
        }
    }

    private void OnDisable()
    {
        if (damageble != null)
        {
            damageble.OnDie -= OnPlayerDie;
        }
    }

    private void OnPlayerDie()
    {
        ChangeState<DeathState>();
    }

    // Метод для получения состояния по типу
    public T GetState<T>() where T : GameState
    {
        if (states.TryGetValue(typeof(T), out GameState state))
            return state as T;
        return null;
    }

    // Метод для смены состояния по типу
    public void ChangeState<T>() where T : GameState
    {
        T newState = GetState<T>();
        if (newState == null)
        {
            Debug.LogError("Состояние типа " + typeof(T) + " не найдено!");
            return;
        }
        ChangeState(newState);
    }

    // Метод смены состояния по экземпляру
    public void ChangeState(GameState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        if (currentState != null)
        {
            currentState.OnEnter();
        }
    }
}
