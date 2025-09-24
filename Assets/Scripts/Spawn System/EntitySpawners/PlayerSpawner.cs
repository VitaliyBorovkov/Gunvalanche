using System;

using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;

    private const string LOG_PREFIX = "PlayerSpawner";

    public static event Action<PlayerShoot> OnPlayerSpawned;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerSpawnPoint == null)
        {
            Debug.Log($"{LOG_PREFIX}: Player spawn point is not assigned!");
            return;
        }

        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        player.transform.rotation = playerSpawnPoint.rotation;

        var playerLook = player.GetComponentInChildren<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.xRotation = 0f;

            if (playerLook.playerCamera != null)
            {
                playerLook.playerCamera.transform.localRotation = Quaternion.identity;
            }
        }

        var switcher = player.GetComponent<PlayerSwitchWeapon>();
        var iconManager = FindObjectOfType<WeaponIconManager>();
        var keyProvider = FindObjectOfType<WeaponKeyProvider>();

        if (switcher != null)
        {
            if (keyProvider != null)
            {
                switcher.SetKeyProvider(keyProvider);
                //Debug.Log($"{LOG_PREFIX}: Injected KeyProvider '{keyProvider.name}' into PlayerSwitchWeapon.");
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX}: WeaponKeyProvider not found in the scene!");
            }

            if (iconManager != null)
            {
                switcher.OnWeaponIconKeyChanged += (keyProvider) =>
                {
                    if (string.IsNullOrEmpty(keyProvider))
                    {
                        iconManager.ClearIcon();
                    }
                    else
                    {
                        iconManager.ShowIconForKey(keyProvider);
                    }
                };

                //Debug.Log($"{LOG_PREFIX}: Subscribed PlayerSwitchWeapon.OnWeaponIconKeyChanged -> " +
                //        $"WeaponIconManager '{iconManager.name}'.");
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX}: WeaponIconManager not found in the scene!");
            }
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}: PlayerSwitchWeapon not found on spawned player.");
        }

        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
        if (playerShoot == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: The player is missing a component PlayerShoot!");
        }
        else
        {
            OnPlayerSpawned?.Invoke(playerShoot);
            //Debug.Log($"{LOG_PREFIX}: OnPlayerSpawned invoked.");
        }

        AssignPlayerToEnemies(player.transform);
    }

    private void AssignPlayerToEnemies(Transform playerTransform)
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            enemy.SetPlayerTransform(playerTransform);
        }
    }
}
