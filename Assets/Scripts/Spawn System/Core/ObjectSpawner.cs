using System.Collections.Generic;

using UnityEngine;

public abstract class ObjectSpawner : MonoBehaviour
{
    private const string LOG_PREFIX = "ObjectSpawner";

    [Header("Spawn Settings")]
    [SerializeField] protected Transform[] spawnPoints;
    [SerializeField] protected int maxObjects = 10;
    [SerializeField] protected float spawnInterval = 5f;
    [SerializeField] protected float checkRadius = 0.5f;

    protected SpawnPointManager spawnPointManager;

    private float timer = 0f;
    private bool manualMode = false;

    private void Start()
    {
        InitializeSpawnPointManager();
    }

    protected virtual void Update()
    {
        if (manualMode)
        {
            return;
        }

        if (spawnPointManager != null)
        {
            spawnPointManager.UpdateCooldowns();
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}: spawnPointManager равен null. Кулдауны не обновляются!");
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval && CountActiveObjects() < maxObjects)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    public void SetManualMode(bool enabled)
    {
        manualMode = enabled;
        timer = 0f;
    }

    public bool TrySpawnNow()
    {
        if (CountActiveObjects() >= maxObjects)
        {
            return false;
        }

        SpawnObject();
        return true;
    }

    protected virtual int CountActiveObjects()
    {
        return 0;
    }

    private void InitializeSpawnPointManager()
    {
        spawnPointManager = FindObjectOfType<SpawnPointManager>();
        if (spawnPointManager == null)
        {
            Debug.Log($"{LOG_PREFIX}: {GetType().Name}: SpawnPointManager не найден на сцене!");
            enabled = false;
            return;
        }

        spawnPointManager.InitializeSpawnPoint(spawnPoints);
    }

    protected abstract void SpawnObject();

    protected Transform GetAvailableSpawnPoint(SpawnPointManager spawnPointManager, float checkRadius, System.Type itemType)
    {
        if (!CheckerToNull.CheckArrayNotEmpty(spawnPoints, nameof(spawnPoints)))
        {
            Debug.Log($"{LOG_PREFIX}: {GetType().Name}: Spawn points are not configured.");
            return null;
        }

        List<Transform> availablePoints = new List<Transform>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[i];
            if (spawnPointManager.IsPointAvailable(point, checkRadius, itemType))
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0)
        {
            return null;
        }

        return availablePoints[Random.Range(0, availablePoints.Count)];
    }
}
