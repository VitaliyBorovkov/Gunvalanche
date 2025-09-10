using UnityEngine;

public class ActionMapRequester : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    private string pendingActionMap;

    private void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(PlayerShoot playerShoot)
    {
        if (playerShoot == null)
        {
            return;
        }

        InputManager inputManager = playerShoot.GetComponent<InputManager>();
        if (inputManager != null)
        {
            SetInputManager(inputManager);
            //Debug.Log($"ActionMapRequester: received InputManager from spawned player ({inputManager.gameObject.name}).");
        }
    }

    public void SetInputManager(InputManager inputManager)
    {
        if (inputManager == null)
        {
            Debug.LogWarning("ActionMapRequester: SetInputManager called with null.");
            return;
        }

        this.inputManager = inputManager;

        //Debug.Log($"ActionMapRequester: SetInputManager -> {inputManager.gameObject.name}");

        if (!string.IsNullOrEmpty(pendingActionMap))
        {
            RequestActionMap(pendingActionMap);
        }
    }

    public void RequestGameplayMap()
    {
        RequestActionMap("GameplayActionMap");
    }

    public void RequestUIMap()
    {
        RequestActionMap("UIActionMap");
    }

    private void RequestActionMap(string mapName)
    {
        if (inputManager != null)
        {
            if (mapName == "GameplayActionMap")
            {
                inputManager.SwitchToGameplayActionMap();
            }
            else if (mapName == "UIActionMap")
            {
                inputManager.SwitchToUIActionMap();
            }
            pendingActionMap = null;
        }
        else
        {
            pendingActionMap = mapName;
            Debug.Log($"GameStateMachine: InputManager not ready, pending action map '{mapName}'.");
        }
    }
}
