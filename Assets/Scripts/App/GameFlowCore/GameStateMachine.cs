using System;
using System.Collections.Generic;

using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PauseUI pauseUI;

    private GameStateContext gameStateContext;
    private readonly Dictionary<Type, IGameState> states = new();
    private IGameState current;

    private PlayerDeathHandler playerDeathHandler;

    private bool subscribedToStatic = false;
    private bool subscribedToSpawner = false;

    private string pendingActionMap = null;

    private void Awake()
    {
        gameStateContext = new GameStateContext(inputManager, inputHandler, gameOverUI, pauseUI);

        gameStateContext.RequestGameplayMap = RequestGameplayMap;
        gameStateContext.RequestUIMap = RequestUIMap;

        states[typeof(GameplayState)] = new GameplayState(gameStateContext);
        states[typeof(GameOverState)] = new GameOverState(gameStateContext);
        states[typeof(PausedState)] = new PausedState(gameStateContext);

        PlayerDeathHandler.PlayerDied += HandlePlayerDeath;
        subscribedToStatic = true;

        PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
        subscribedToSpawner = true;

        playerDeathHandler = FindObjectOfType<PlayerDeathHandler>(true);
        if (playerDeathHandler != null)
        {
            RegisterPlayerDeathHandler(playerDeathHandler);
        }
    }

    private void Start()
    {
        ChangeState<GameplayState>();
    }

    private void Update()
    {
        current?.UpdateState();
    }

    private void OnDestroy()
    {
        if (playerDeathHandler != null)
        {
            playerDeathHandler.Died -= HandlePlayerDeath;
            playerDeathHandler = null;
        }

        if (subscribedToStatic)
        {
            PlayerDeathHandler.PlayerDied -= HandlePlayerDeath;
            subscribedToStatic = false;
        }

        if (subscribedToSpawner)
        {
            PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawned;
            subscribedToSpawner = false;
        }
    }

    private void OnPlayerSpawned(PlayerShoot playerShoot)
    {
        if (playerShoot == null)
        {
            return;
        }

        var playerDeathHandler = playerShoot.GetComponent<PlayerDeathHandler>();
        if (playerDeathHandler != null)
        {
            RegisterPlayerDeathHandler(playerDeathHandler);
        }

        var input = playerShoot.GetComponent<InputManager>();
        if (input != null)
        {
            inputManager = input;

            if (!string.IsNullOrEmpty(pendingActionMap))
            {
                RequestActionMap(pendingActionMap);
            }
        }
        else
        {
            Debug.LogWarning("GameStateMachine: spawned player has no InputManager.");
        }
    }

    private void RegisterPlayerDeathHandler(PlayerDeathHandler playerDeathHandler)
    {
        if (playerDeathHandler == null) return;

        if (playerDeathHandler != null)
        {
            playerDeathHandler.Died -= HandlePlayerDeath;
        }

        //playerDeathHandler.Died -= HandlePlayerDeath;
        playerDeathHandler.Died += HandlePlayerDeath;
    }

    public void ChangeState<T>() where T : IGameState
    {
        var next = states[typeof(T)];
        ChangeState(next);
    }

    private void ChangeState(IGameState next)
    {
        current?.ExitState();
        current = next;
        current.EnterState();
        Debug.Log($"GameStateMachine: state -> {current.GetType().Name}");
    }

    public void ToGameplay()
    {
        ChangeState<GameplayState>();
    }

    public void ToGameOver()
    {
        ChangeState<GameOverState>();
    }

    public void ToPause()
    {
        ChangeState<PausedState>();
    }

    private void HandlePlayerDeath()
    {
        if (current != null && current.GetType() == typeof(GameOverState))
        {
            Debug.Log("GameStateMachine: already in GameOverState, ignoring duplicate death.");
            return;
        }

        Debug.Log("GameStateMachine: HandlePlayerDeath -> ToGameOver");
        ToGameOver();
    }

    public void RequestGameplayMap() => RequestActionMap("GameplayActionMap");
    public void RequestUIMap() => RequestActionMap("UIActionMap");

    private void RequestActionMap(string mapName)
    {
        if (inputManager != null)
        {
            if (mapName == "GameplayActionMap") inputManager.SwitchToGameplayActionMap();
            else if (mapName == "UIActionMap") inputManager.SwitchToUIActionMap();
            pendingActionMap = null;
        }
        else
        {
            pendingActionMap = mapName;
            Debug.Log($"GameStateMachine: InputManager not ready, pending action map '{mapName}'.");
        }
    }
}