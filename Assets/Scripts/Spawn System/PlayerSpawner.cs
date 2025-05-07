using System;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerSpawnPoint == null)
        {
            Debug.Log("Player spawn point is not assigned!");
            return;
        }

        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        player.transform.position = playerSpawnPoint.position;
        player.transform.rotation = playerSpawnPoint.rotation;

        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
        if (playerShoot == null )
        {
            Debug.LogWarning("PlayerSpawner: У игрока отсутствует компонент PlayerShoot!");
            Debug.LogWarning("PlayerSpawner: У игрока отсутствует компонент PlayerShoot!");
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
