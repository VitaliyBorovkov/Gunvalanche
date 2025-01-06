using UnityEngine;

public abstract class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]

    [SerializeField] protected Transform[] spawnPoints;
    [SerializeField] protected int maxObjects = 10;
    [SerializeField] protected float spawnInterval = 5f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && CountActiveObjects() < maxObjects)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    protected virtual int CountActiveObjects()
    {
        return FindObjectsOfType<HealthController>().Length;
    }

    protected abstract void SpawnObject();
}
