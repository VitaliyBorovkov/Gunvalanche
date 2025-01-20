using System;
using UnityEngine;

public class AmmoBoxSpawner : ObjectSpawner
{
    [Header("Ammo Box Settings")]

    [SerializeField] private ObjectPool pistolAmmoPool;
    [SerializeField] private ObjectPool riffleAmmoPool;
    [SerializeField] private ObjectPool rocketLauncherAmmoPool;

    protected override void SpawnObject()
    {
        Transform spawnPoint = GetAvailableSpawnPoint(spawnPointManager, checkRadius, typeof(AmmoBox));
        if (spawnPoint == null)
        {
            return;
        }

        ObjectPool selectedPool = GetRandomAmmoPool();
        GameObject spawnedAmmoBox = selectedPool.Spawn(spawnPoint.position, Quaternion.identity);
        AmmoBox ammoBox = spawnedAmmoBox.GetComponent<AmmoBox>();
        if (ammoBox != null)
        {
            ammoBox.SetAmmoBoxPool(selectedPool);
            spawnPointManager.OccupyPoint(spawnPoint, "AmmoBox");
        }
        else
        {
            Debug.LogWarning($"AmmoBoxSpawner: У объекта {spawnedAmmoBox.name} отсутствует компонент AmmoBox!");
        }
    }

    private ObjectPool GetRandomAmmoPool()
    {
        int randomIndex = UnityEngine.Random.Range(0, 3);
        return randomIndex switch
        {
            0 => pistolAmmoPool,
            1 => riffleAmmoPool,
            2 => rocketLauncherAmmoPool,
            _ => pistolAmmoPool,
        };
    }

    protected override int CountActiveObjects()
    {
        return pistolAmmoPool.CountActiveObjects() + riffleAmmoPool.CountActiveObjects() + 
            rocketLauncherAmmoPool.CountActiveObjects();
    }
}

