using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private ObjectPool objectPool;

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
        if (playerShoot != null && objectPool != null)
        {
            playerShoot.SetObjectPool(objectPool);
        }

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            enemy.SetPlayerTransform(player.transform);
        }

        Debug.Log("PlayerSpawner —сылки на игрока обновлены у всех врагов.");
    }
}
