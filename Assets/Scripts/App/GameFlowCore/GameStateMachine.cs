using System;
using System.Collections.Generic;

using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PauseUI pauseUI;

    private GameStateContext gameStateContext;
    private readonly Dictionary<Type, IGameState> states = new();
    private IGameState currentState;

    private PlayerDeathHandler playerDeathHandler;

    private bool subscribedToStatic = false;
    private bool subscribedToSpawner = false;

    private void Awake()
    {
        gameStateContext = new GameStateContext(inputHandler, inputManager, gameOverUI, pauseUI);

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
        currentState?.UpdateState();
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
        var input = playerShoot.GetComponent<InputManager>();
        if (input != null)
        {
            inputManager = input;
            gameStateContext.SetInputManager(inputManager);
            Debug.Log("GameStateMachine: registered InputManager from PlayerSpawner.");

            if (currentState != null && currentState.GetType() == typeof(GameplayState))
            {
                inputManager.SwitchToGameplayActionMap();
            }
        }
        else
        {
            Debug.LogWarning("GameStateMachine: spawned player has no InputManager.");
        }

        if (playerShoot == null)
        {
            return;
        }

        var playerDeathHandler = playerShoot.GetComponent<PlayerDeathHandler>();
        if (playerDeathHandler == null)
        {
            Debug.LogWarning("GameStateMachine: spawned player has no PlayerDeathHandler.");
            return;
        }

        RegisterPlayerDeathHandler(playerDeathHandler);
    }

    private void RegisterPlayerDeathHandler(PlayerDeathHandler playerDeathHandler)
    {
        if (playerDeathHandler == null) return;

        if (playerDeathHandler != null)
        {
            playerDeathHandler.Died -= HandlePlayerDeath;
        }

        playerDeathHandler.Died -= HandlePlayerDeath;
        playerDeathHandler.Died += HandlePlayerDeath;
    }

    public void ChangeState<T>() where T : IGameState
    {
        var next = states[typeof(T)];
        ChangeState(next);
    }

    private void ChangeState(IGameState next)
    {
        currentState?.ExitState();
        currentState = next;
        currentState.EnterState();
        //Debug.Log($"GameStateMachine: state -> {current.GetType().Name}");
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
        if (currentState != null && currentState.GetType() == typeof(GameOverState))
        {
            Debug.Log("GameStateMachine: already in GameOverState, ignoring duplicate death.");
            return;
        }

        Debug.Log("GameStateMachine: HandlePlayerDeath -> ToGameOver");
        ToGameOver();
    }
}