using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterPool(string poolID, ObjectPool pool)
    {
        if (!pools.ContainsKey(poolID))
        {
            pools.Add(poolID, pool);
        }
    }

    public ObjectPool GetPool(string poolID)
    {
        if (pools.ContainsKey(poolID))
        {
            return pools[poolID];
        }
        else
        {
            Debug.LogWarning($"ObjectPoolManager: ѕул с ID {poolID} не найден!");
            return null;
        }
    }
}
