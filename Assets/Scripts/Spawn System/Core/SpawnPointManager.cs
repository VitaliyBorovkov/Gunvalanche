using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] private float minSpawnCooldown = 5f;

    private Dictionary<Transform, float> spawnCooldown;

    private void Awake()
    {
        spawnCooldown = new Dictionary<Transform, float>();
    }

    public void InitializeSpawnPoint(Transform[] spawnPoints)
    {
        foreach (Transform point in spawnPoints)
        {
            if (!spawnCooldown.ContainsKey(point))
            {
                spawnCooldown[point] = 0f;
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

        Collider[] colliders = Physics.OverlapSphere(spawnPoint.position, checkRadius);
        foreach (Collider col in colliders)
        {
            if (col.GetComponent(itemType) != null)
            {
                return false;
            }
        }

        return true;
    }

    public void SetCooldown(Transform spawnPoint)
    {
        if (spawnCooldown.ContainsKey(spawnPoint))
        {
            spawnCooldown[spawnPoint] = minSpawnCooldown;
        }
    }
}
