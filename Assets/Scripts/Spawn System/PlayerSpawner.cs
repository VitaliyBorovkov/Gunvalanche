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

        //player.transform.position = playerSpawnPoint.position;
        //player.transform.rotation = playerSpawnPoint.rotation;

        var switcher = player.GetComponent<PlayerSwitchWeapon>();
        if (switcher != null)
        {
            var iconManager = FindObjectOfType<WeaponIconManager>();
            if (iconManager != null)
            {
                switcher.SetWeaponIconManager(iconManager);
                Debug.Log($"{LOG_PREFIX}: Assigned WeaponIconManager '{iconManager.name}' to spawned player.");
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX}: WeaponIconManager not found in the scene!");
            }
        }

        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
        if (playerShoot == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: The player is missing a component PlayerShoot!");
        }
        else
        {
            //AmmoUIHandler ammoUIHandler = FindObjectOfType<AmmoUIHandler>();
            //if (ammoUIHandler != null)
            //{
            //    ammoUIHandler.SetPlayerShoot(playerShoot);
            //}
            //else
            //{
            //    Debug.LogWarning("AmmoUIHandler not found in the scene!");
            //}

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
