using UnityEngine;
using UnityEngine.InputSystem;

public class Pauser : MonoBehaviour
{
    [SerializeField] private GameStateMachine gameStateMachine;
    [SerializeField] private InputManager inputManager;

    private bool subscribedToSpawner;
    private bool subscribedToInput;

    private void Awake()
    {
        if (gameStateMachine == null)
        {
            gameStateMachine = FindObjectOfType<GameStateMachine>();
        }
    }

    private void Start()
    {
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>(true);
        }

        if (inputManager != null)
        {
            Register(inputManager);
        }
        else
        {
            PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
            subscribedToSpawner = true;
            Debug.Log("Pauser: waiting for player spawn to obtain InputManager.");
        }
    }

    private void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawned;
        Unregister();
    }

    private void OnPlayerSpawned(PlayerShoot playerShoot)
    {
        if (playerShoot == null)
        {
            return;
        }

        var inputManager = playerShoot.GetComponent<InputManager>();
        if (inputManager == null)
        {
            Debug.LogWarning("Pauser: spawned player has no InputManager.");
            return;
        }

        Register(inputManager);
        if (subscribedToSpawner)
        {
            PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawned;
            subscribedToSpawner = false;
        }
    }

    private void Register(InputManager input)
    {
        if (inputManager == null || subscribedToInput && inputManager == input)
        {
            return;
        }

        Unregister();

        inputManager = input;
        var playerInpyt = inputManager.GetComponent<PlayerInput>();
        if (playerInpyt == null || playerInpyt.actions == null)
        {
            Debug.LogWarning("Pauser: PlayerInput or actions asset missing on InputManager.");
            return;
        }

        var pause = playerInpyt.actions.FindAction("Pause", throwIfNotFound: false);

        if (pause == null)
        {
            Debug.LogWarning("Pauser: no Pause-like action found on PlayerInput (add action named 'Pause' or one of common names).");
            return;
        }

        pause.Enable();
        pause.performed += OnPausePerformed;
        subscribedToInput = true;
        //Debug.Log($"Pauser: subscribed to Pause action '{pause.name}' on '{inputManager.gameObject.name}'.");
    }

    private void Unregister()
    {
        if (!subscribedToInput || inputManager == null)
        {
            subscribedToInput = false; return;
        }

        var playerInpyt = inputManager.GetComponent<PlayerInput>();
        if (playerInpyt?.actions != null)
        {
            var pause = playerInpyt.actions.FindAction("Pause", throwIfNotFound: false);
            if (pause != null)
            {
                pause.performed -= OnPausePerformed;
            }
        }

        subscribedToInput = false;
        //Debug.Log("Pauser: unsubscribed from Pause action.");
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Pauser: Pause action triggered.");
        if (Time.timeScale > 0.5f)
        {
            gameStateMachine?.ToPause();
            //Debug.Log("Pauser: Paused.");
        }
        else
        {
            gameStateMachine?.ToGameplay();
            //Debug.Log("Pauser: Resumed.");
        }
    }
}
