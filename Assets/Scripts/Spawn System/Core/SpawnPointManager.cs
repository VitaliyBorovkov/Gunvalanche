using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] private float minSpawnCooldown = 5f;

    private Dictionary<Transform, float> spawnCooldown;
    private Dictionary<Transform, string> occupiedPoints;

    public static SpawnPointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"SpawnPointManager: Найден ещё один экземпляр SpawnPointManager на объекте {gameObject.name}. Он будет уничтожен.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        spawnCooldown = new Dictionary<Transform, float>();
        occupiedPoints = new Dictionary<Transform, string>();
    }

    public void InitializeSpawnPoint(Transform[] spawnPoints)
    {
        foreach (Transform point in spawnPoints)
        {
            if (!spawnCooldown.ContainsKey(point))
            {
                spawnCooldown[point] = 0f;
                occupiedPoints[point] = null;
            }
        }
    }

    public void UpdateCooldowns()
    {
        List<Transform> keys = new List<Transform>(spawnCooldown.Keys);

        foreach(Transform spawnPoint in keys)
        {
            if (spawnCooldown[spawnPoint] > 0)
            {
                spawnCooldown[spawnPoint] -= Time.deltaTime;
            }
            else
            {
                if (occupiedPoints[spawnPoint] != null)
                {
                    spawnCooldown[spawnPoint] = 0f;
                }
            }
        }
    }

    public bool IsPointAvailable(Transform spawnPoint, float checkRadius, System.Type itemType)
    {
        if (spawnCooldown.ContainsKey(spawnPoint) && spawnCooldown[spawnPoint] > 0)
        {
            Debug.Log($"SpawnPointManager: Точка {spawnPoint.name} не доступна. Кулдаун ещё активен: {spawnCooldown[spawnPoint]:F2} сек.");
            return false;
        }

        if (occupiedPoints.ContainsKey(spawnPoint) && occupiedPoints[spawnPoint] != null)
        {
            Debug.Log($"SpawnPointManager occupiedPoints: Точка {spawnPoint.name} занята объектом {occupiedPoints[spawnPoint]}.");
            return false;
        }

        Collider[] colliders = Physics.OverlapSphere(spawnPoint.position, checkRadius);
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<CollectibleItems>() != null)
            {
                Debug.Log($"SpawnPointManager: Точка {spawnPoint.name} занята объектом {col.gameObject.name}, тип: {col.GetComponent<CollectibleItems>().GetType().Name}.");
                return false;
            }
        }
        Debug.Log($"SpawnPointManager: Точка {spawnPoint.name} доступна для спавна.");
        return true;
    }

    public void OccupyPoint(Transform spawnPoint, string objectType)
    {
        if (occupiedPoints.ContainsKey(spawnPoint))
        {
            occupiedPoints[spawnPoint] = objectType;
        }
    }

    public void ReleasePoint(Transform spawnPoint)
    {
        if (occupiedPoints.ContainsKey(spawnPoint))
        {
            Debug.Log($"SpawnPointManager: Освобождаем точку {spawnPoint.name}.");
            occupiedPoints[spawnPoint] = null;
        }
        else
        {
            Debug.LogWarning($"SpawnPointManager: Попытка освободить несуществующую точку {spawnPoint.name}.");
        }
    }

    public void SetCooldown(Transform spawnPoint)
    {
        if (spawnCooldown.ContainsKey(spawnPoint))
        {
            Debug.Log($"SpawnPointManager: Устанавливаем кулдаун для точки {spawnPoint.name}, время: {minSpawnCooldown} сек.");
            spawnCooldown[spawnPoint] = minSpawnCooldown;
        }
    }
}
