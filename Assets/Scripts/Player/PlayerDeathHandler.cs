using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeathHandler : MonoBehaviour
{
    public static event Action PlayerDied;

    [SerializeField] private HealthController playerHealth;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private PlayerReload playerReload;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameOverUI gameOverUI;

    [SerializeField] private float fadeOutDuration = 0.8f;

    private bool handled = false;

    private void Reset()
    {
        playerHealth = GetComponent<HealthController>();
        playerShoot = GetComponent<PlayerShoot>();
        playerReload = GetComponent<PlayerReload>();
        playerInput = GetComponent<PlayerInput>();
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

        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (gameOverUI == null)
        {
            gameOverUI = FindObjectOfType<GameOverUI>(true);

            if (gameOverUI == null)
            {
                var tagged = GameObject.FindGameObjectWithTag("GameOverUI");
                if (tagged != null)
                {
                    gameOverUI = tagged.GetComponent<GameOverUI>();
                }

                Debug.LogWarning("PlayerDeathHandler: GameOverUI not found in the scene!");
            }
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

    private void OnPlayerDied()
    {
        if (handled)
        {
            return;
        }
        handled = true;

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        if (playerShoot != null)
        {
            playerShoot.enabled = false;
        }

        if (playerReload != null)
        {
            playerReload.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerDied?.Invoke();

        Time.timeScale = 0f;

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOverScreen(fadeOutDuration);
        }
        else
        {
            Debug.LogWarning("PlayerDeathHandler: GameOverUI is not assigned!");
        }
    }
}
