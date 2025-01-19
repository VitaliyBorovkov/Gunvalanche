using UnityEngine;

public class EnemySpawner : ObjectSpawner
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ObjectPool enemyPool;

    protected override void SpawnObject()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject spawnedEnemy = enemyPool.Spawn(spawnPoint.position, Quaternion.identity);
        EnemyController enemyController = spawnedEnemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                enemyController.SetPlayerTransform(player.transform);
                //Debug.Log($"{enemyController.gameObject.name} получил ссылку на игрока: {player.transform.position}");
            }
            else
            {
                Debug.LogWarning("EnemySpawner: Игрок не найден при спавне врага!");
            }
        }

        EnemyHealthController enemyHealthController = spawnedEnemy.GetComponent<EnemyHealthController>();
        if (enemyHealthController != null)
        {
            enemyHealthController.SetEnemyPool(enemyPool);
        }

        ISpawnable spawnable = spawnedEnemy.GetComponent<ISpawnable>();
        spawnable?.OnSpawn();
    }

    protected override int CountActiveObjects()
    {
        return FindObjectsOfType<EnemyController>().Length;
    }
}
