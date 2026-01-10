using System;

using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    public static event Action PlayerDied;
    public event Action Died;

    [SerializeField] private HealthController playerHealth;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private PlayerReload playerReload;
    [SerializeField] private InputHandler inputHandler;

    private bool handled = false;

    private void Reset()
    {
        playerHealth = GetComponent<HealthController>();
        playerShoot = GetComponent<PlayerShoot>();
        playerReload = GetComponent<PlayerReload>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<HealthController>();
        }

        if (playerShoot == null)
        {
            playerShoot = GetComponent<PlayerShoot>();
        }

        if (playerReload == null)
        {
            playerReload = GetComponent<PlayerReload>();
        }

        if (inputHandler == null)
        {
            inputHandler = GetComponent<InputHandler>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDied += OnPlayerDied;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDied -= OnPlayerDied;
        }
    }

    private void OnPlayerDied(HealthController healthController)
    {
        if (handled)
        {
            return;
        }
        handled = true;

        inputHandler?.SetEnabled(false);
        playerShoot?.StopFiring();
        playerReload?.CancelReload();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Died?.Invoke();
        PlayerDied?.Invoke();

        Time.timeScale = 0f;
    }
}
