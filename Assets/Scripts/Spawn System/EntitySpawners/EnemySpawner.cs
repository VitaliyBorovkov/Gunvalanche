using System;

using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : ObjectSpawner
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ObjectPool enemyPool;
    [SerializeField] private ObjectPool damageTextPool;

    public event Action<HealthController> OnEnemySpawned;

    protected override void SpawnObject()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Quaternion spawnRotation = Quaternion.identity;
        if (playerTransform != null)
        {
            Vector3 dir = playerTransform.position - spawnPoint.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                spawnRotation = Quaternion.LookRotation(dir);
        }
        GameObject spawnedEnemy = enemyPool.Spawn(spawnPoint.position, spawnRotation);

        // Warp the NavMeshAgent to the exact spawn position so it starts on the NavMesh
        var agent = spawnedEnemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(spawnPoint.position);
            agent.ResetPath();
            spawnedEnemy.transform.rotation = spawnRotation;
        }

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
            enemyHealthController.SetDamageTextPool(damageTextPool);
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
