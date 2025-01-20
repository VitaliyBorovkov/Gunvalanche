using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] private float minSpawnCooldown = 5f;

    private Dictionary<Transform, float> spawnCooldown;
    private Dictionary<Transform, string> occupiedPoints;

    public static SpawnPointManager Instance { get; private set; }

    private void Awake()
    {
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

        for (int i = 0; i < keys.Count; i++)
        {
            if (spawnCooldown[keys[i]] > 0)
            {
                spawnCooldown[keys[i]] -= Time.deltaTime;
            }
        }
    }

    public bool IsPointAvailable(Transform spawnPoint, float checkRadius, System.Type itemType)
    {
        if (spawnCooldown.ContainsKey(spawnPoint) && spawnCooldown[spawnPoint] > 0)
        {
            return false;
        }

        if (occupiedPoints.ContainsKey(spawnPoint) && occupiedPoints[spawnPoint] != null)
        {
            return false;
        }

        Collider[] colliders = Physics.OverlapSphere(spawnPoint.position, checkRadius);
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<CollectibleItems>() != null)
            {
                return false;
            }
        }

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
            occupiedPoints[spawnPoint] = null;
        }
    }

    public void SetCooldown(Transform spawnPoint)
    {
        if (spawnCooldown.ContainsKey(spawnPoint))
        {
            spawnCooldown[spawnPoint] = minSpawnCooldown;
        }
    }
}
