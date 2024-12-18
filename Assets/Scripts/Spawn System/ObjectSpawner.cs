using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] objectPrefabs;
    [SerializeField] private int maxObjects = 10;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && CountActiveObjects() < maxObjects)
        {
            SpawnRandomObject();
            timer = 0f;
        }
    }

    private void SpawnRandomObject()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];

        GameObject spawnedObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        EnemyController enemyController = spawnedObject.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                enemyController.SetPlayerTransform(player.transform);
            }
        }

        ISpawnable spawnable = spawnedObject.GetComponent<ISpawnable>();
        spawnable?.OnSpawn();
    }

    private int CountActiveObjects()
    {
        return FindObjectsOfType<HealthController>().Length;
    }

    //public void SetPlayerTransform(Transform player)
    //{
    //    playerTransform = player;
    //}
}
