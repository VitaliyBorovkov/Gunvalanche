using System;

using UnityEngine;

public class PlayerDeathObserver : MonoBehaviour
{
    [SerializeField] private GameStateMachine gameStateMachine;

    public event Action OnPlayerDied;

    private bool subscribedToStaticEvent = false;
    private PlayerDeathHandler playerDeathHandler;

    private void Awake()
    {
        PlayerDeathHandler.PlayerDied += HandleStaticPlayerDeath;
        subscribedToStaticEvent = true;

        playerDeathHandler = FindObjectOfType<PlayerDeathHandler>(true);
        if (playerDeathHandler != null)
        {
            RegisterPlayerDeathHandler(playerDeathHandler);
        }

        if (gameStateMachine == null)
        {
            gameStateMachine = FindObjectOfType<GameStateMachine>();
            if (gameStateMachine != null)
            {
                Debug.Log($"PlayerDeathObserver: found GameStateMachine -> {gameStateMachine.gameObject.name}");
            }
        }
    }

    private void Start()
    {
        PlayerSpawner.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDestroy()
    {
        if (subscribedToStaticEvent)
        {
            PlayerDeathHandler.PlayerDied -= HandleStaticPlayerDeath;
            subscribedToStaticEvent = false;
        }

        if (playerDeathHandler != null)
        {
            playerDeathHandler.Died -= HandleInstancePlayerDeath;
            playerDeathHandler = null;
        }

        PlayerSpawner.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandleStaticPlayerDeath()
    {
        //Debug.Log("PlayerDeathObserver: static PlayerDied invoked.");
        OnPlayerDied?.Invoke();
        RequestGameOver();
    }

    private void HandleInstancePlayerDeath()
    {
        //Debug.Log("PlayerDeathObserver: instance Died invoked.");
        OnPlayerDied?.Invoke();
        RequestGameOver();
    }

    private void HandlePlayerSpawned(PlayerShoot playerShoot)
    {
        if (playerShoot == null)
        {
            return;
        }

        PlayerDeathHandler playerDeathHandler = playerShoot.GetComponent<PlayerDeathHandler>();
        if (playerDeathHandler != null)
        {
            RegisterPlayerDeathHandler(playerDeathHandler);
            //Debug.Log($"PlayerDeathObserver: registered instance death handler from spawned player ({playerShoot.gameObject.name}).");
        }
    }

    private void RegisterPlayerDeathHandler(PlayerDeathHandler playerDeathHandler)
    {
        if (this.playerDeathHandler != null)
        {
            this.playerDeathHandler.Died -= HandleInstancePlayerDeath;
        }

        this.playerDeathHandler = playerDeathHandler;
        this.playerDeathHandler.Died += HandleInstancePlayerDeath;
    }

    private void RequestGameOver()
    {
        if (gameStateMachine == null)
        {
            gameStateMachine = FindObjectOfType<GameStateMachine>(true);
            if (gameStateMachine == null)
            {
                Debug.LogWarning("PlayerDeathObserver: GameStateMachine not found, cannot request GameOver.");
                return;
            }
        }

        //Debug.Log("PlayerDeathObserver: requesting GameOver.");
        gameStateMachine.ToGameOver();
    }
}
