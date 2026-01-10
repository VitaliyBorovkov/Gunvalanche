using System;
using System.Collections;

using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private const string LOG_PREFIX = "WaveManager";

    [SerializeField] private LevelWavesConfig levelWavesConfig;
    [SerializeField] private EnemySpawner[] enemySpawners;

    public event Action OnAllWavesCompleted;

    public bool AreAllWavesCompleted { get; private set; }

    private int currentWaveIndex;
    private int totalEnemiesInWave;
    private int spawnedEnemiesInWave;
    private int aliveEnemies = 0;
    private bool waveFinishedSpawning;

    private void Start()
    {
        AreAllWavesCompleted = false;
        StartCoroutine(RunLevel());
    }

    private void OnDisable()
    {
        UnsubscribeFromAllSpawners();
    }

    private EnemySpawner GetSpawner(int index)
    {
        if (enemySpawners == null || index < 0 || index >= enemySpawners.Length)
        {
            return null;
        }

        return enemySpawners[index];
    }

    private IEnumerator RunLevel()
    {
        if (levelWavesConfig == null || levelWavesConfig.waves == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: LevelWavesConfig is NULL or waves list is NULL.");
            yield break;
        }

        for (int i = 0; i < levelWavesConfig.waves.Count; i++)
        {
            var wave = levelWavesConfig.waves[i];

            if (wave.startDelay > 0f)
            {
                yield return new WaitForSeconds(wave.startDelay);
            }

            foreach (var waveSpawn in wave.waveSpawns)
            {
                var spawner = GetSpawner(waveSpawn.spawnerIndex);
                if (spawner != null)
                {
                    spawner.SetManualMode(true);
                }
            }

            currentWaveIndex = i + 1;
            spawnedEnemiesInWave = 0;
            aliveEnemies = 0;

            totalEnemiesInWave = 0;
            foreach (var ws in wave.waveSpawns)
            {
                totalEnemiesInWave += ws.enemyCountInWave;
            }

            Debug.Log($"{LOG_PREFIX}: Wave {currentWaveIndex} started. " + $"{LOG_PREFIX}: {totalEnemiesInWave}");

            waveFinishedSpawning = false;

            foreach (var waveSpawn in wave.waveSpawns)
            {
                var spawner = GetSpawner(waveSpawn.spawnerIndex);
                if (spawner != null)
                {
                    spawner.OnEnemySpawned += RegisterEnemy;
                }
            }

            foreach (var waveSpawn in wave.waveSpawns)
            {
                var spawner = GetSpawner(waveSpawn.spawnerIndex);
                if (spawner == null)
                {
                    Debug.LogWarning($"{LOG_PREFIX}: Invalid spawnerIndex: {waveSpawn.spawnerIndex}");
                    continue;
                }

                for (int j = 0; j < waveSpawn.enemyCountInWave; j++)
                {
                    bool spawned = spawner.TrySpawnNow();
                    if (spawned)
                    {
                        spawnedEnemiesInWave++;
                        Debug.Log($"{LOG_PREFIX}: Wave {currentWaveIndex}: " +
                            $"{LOG_PREFIX}: {spawnedEnemiesInWave}/{totalEnemiesInWave}");
                    }

                    yield return new WaitForSeconds(waveSpawn.timeBetweenSpawnEnemyInWave);
                }
            }

            waveFinishedSpawning = true;

            yield return new WaitUntil(() => waveFinishedSpawning && aliveEnemies <= 0);
            Debug.Log($"{LOG_PREFIX}: Wave {currentWaveIndex} completed.");

            foreach (var waveSpawn in wave.waveSpawns)
            {
                var spawner = GetSpawner(waveSpawn.spawnerIndex);
                if (spawner != null)
                {
                    spawner.OnEnemySpawned -= RegisterEnemy;
                }
            }
        }

        AreAllWavesCompleted = true;
        Debug.Log($"{LOG_PREFIX}: All waves completed.");

        OnAllWavesCompleted?.Invoke();
    }

    private void UnsubscribeFromAllSpawners()
    {
        if (levelWavesConfig == null || levelWavesConfig.waves == null)
        {
            return;
        }

        foreach (var wave in levelWavesConfig.waves)
        {

            if (wave.waveSpawns == null)
            {
                continue;
            }

            foreach (var waveSpawn in wave.waveSpawns)
            {
                var spawner = GetSpawner(waveSpawn.spawnerIndex);
                if (spawner != null)
                {
                    spawner.OnEnemySpawned -= RegisterEnemy;
                }
            }
        }
    }

    private void RegisterEnemy(HealthController healthController)
    {
        if (healthController == null)
        {
            return;
        }

        aliveEnemies++;
        healthController.OnDied += OnEnemyDied;
    }

    private void OnEnemyDied(HealthController healthController)
    {
        if (healthController != null)
        {
            healthController.OnDied -= OnEnemyDied;
        }

        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);

        Debug.Log($"{LOG_PREFIX}: Wave {currentWaveIndex}: Enemy died. Remaining: {aliveEnemies}");
    }
}
