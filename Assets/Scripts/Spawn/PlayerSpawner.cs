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

        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
        if (playerShoot != null && objectPool != null)
        {
            playerShoot.SetObjectPool(objectPool);
        }
    }
}
