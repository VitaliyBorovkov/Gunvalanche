using System;

using UnityEngine;

public class EnemySpawner : ObjectSpawner
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ObjectPool enemyPool;

    public event Action<HealthController> OnEnemySpawned;

    protected override void SpawnObject()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
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
                Debug.LogWarning("EnemySpawner: Player not found when enemy spawns!");
            }
        }

        EnemyHealthController enemyHealthController = spawnedEnemy.GetComponent<EnemyHealthController>();
        if (enemyHealthController != null)
        {
            enemyHealthController.SetEnemyPool(enemyPool);
        }

        ISpawnable spawnable = spawnedEnemy.GetComponent<ISpawnable>();
        spawnable?.OnSpawn();

        var health = spawnedEnemy.GetComponent<HealthController>();
        if (health != null)
        {
            OnEnemySpawned?.Invoke(health);
        }
    }

    protected override int CountActiveObjects()
    {
        return enemyPool.CountActiveObjects();
    }
}
