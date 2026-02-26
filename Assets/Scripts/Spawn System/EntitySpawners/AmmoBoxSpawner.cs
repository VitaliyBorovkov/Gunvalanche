using System.Collections.Generic;

using UnityEngine;

public class AmmoBoxSpawner : ObjectSpawner
{
    private const string LOG_PREFIX = "AmmoBoxSpawner";

    [Header("Progression")]
    [SerializeField] private WeaponUnlockManager weaponUnlockManager;

    [Header("Spawn Rules")]
    [SerializeField] private AmmoSpawnRules ammoSpawnRules;

    [Header("Ammo Box Settings")]
    [SerializeField] private ObjectPool pistolAmmoPool;
    [SerializeField] private ObjectPool riffleAmmoPool;
    [SerializeField] private ObjectPool shotgunAmmoPool;
    [SerializeField] private ObjectPool rocketLauncherAmmoPool;

    private readonly List<ObjectPool> availablePoolsCache = new List<ObjectPool>(4);

    private void Reset()
    {
        weaponUnlockManager = FindAnyObjectByType<WeaponUnlockManager>();
    }

    protected override void SpawnObject()
    {
        Transform spawnPoint = GetAvailableSpawnPoint(spawnPointManager, checkRadius, typeof(AmmoBox));
        if (spawnPoint == null)
        {
            //Debug.LogError("AmmoBoxSpawner: Нет доступной точки для спавна.");
            return;
        }

        ObjectPool selectedPool = GetRandomAllowedAmmoPool();

        if (selectedPool == null)
        {
            Debug.LogError("AmmoBoxSpawner:  Selected ammo pool is null. Check spawner references in Inspector!");
            return;
        }

        GameObject spawnedAmmoBox = selectedPool.Spawn(spawnPoint.position, Quaternion.identity);
        if (spawnedAmmoBox == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Pool returned null object on Spawn().");
            return;
        }

        AmmoBox ammoBox = spawnedAmmoBox.GetComponent<AmmoBox>();
        if (ammoBox != null)
        {
            ammoBox.SetAmmoBoxPool(selectedPool);
            ammoBox.SetSpawnPoint(spawnPoint);
            spawnPointManager.OccupyPoint(spawnPoint, "AmmoBox");
        }
        else
        {
            Debug.LogWarning($"AmmoBoxSpawner: Spawned object '{spawnedAmmoBox.name}' has no AmmoBox component!");
        }
    }

    private ObjectPool GetRandomAllowedAmmoPool()
    {
        availablePoolsCache.Clear();

        if (weaponUnlockManager == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: WeaponUnlockManager is not assigned. Falling back to any assigned pools.");

            TryAddPool(GunsType.Pistol, pistolAmmoPool);
            TryAddPool(GunsType.Riffle, riffleAmmoPool);
            TryAddPool(GunsType.Shotgun, shotgunAmmoPool);
            TryAddPool(GunsType.RocketLauncher, rocketLauncherAmmoPool);

            return GetRandomFromCacheOrFallback();
        }

        if (weaponUnlockManager.IsUnlocked(GunsType.Pistol))
        {
            TryAddPool(GunsType.Pistol, pistolAmmoPool);
        }

        if (weaponUnlockManager.IsUnlocked(GunsType.Riffle))
        {
            TryAddPool(GunsType.Riffle, riffleAmmoPool);
        }

        if (weaponUnlockManager.IsUnlocked(GunsType.Shotgun))
        {
            TryAddPool(GunsType.Shotgun, shotgunAmmoPool);
        }

        if (weaponUnlockManager.IsUnlocked(GunsType.RocketLauncher))
        {
            TryAddPool(GunsType.RocketLauncher, rocketLauncherAmmoPool);
        }

        return GetRandomFromCacheOrFallback();
    }

    private void TryAddPool(GunsType gunsType, ObjectPool objectPool)
    {
        if (objectPool == null)
        {
            return;
        }

        bool enabledByRules = ammoSpawnRules == null || ammoSpawnRules.IsAmmoEnabled(gunsType);
        if (!enabledByRules)
        {
            return;
        }

        if (objectPool != null)
        {
            availablePoolsCache.Add(objectPool);
        }
    }

    private ObjectPool GetRandomFromCacheOrFallback()
    {
        if (availablePoolsCache.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePoolsCache.Count);
            return availablePoolsCache[randomIndex];
        }

        if (pistolAmmoPool != null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: No allowed ammo pools found. Falling back to pistolAmmoPool.");
            return pistolAmmoPool;
        }

        Debug.LogError($"{LOG_PREFIX}: No ammo pools assigned and no allowed pools found.");
        return null;
    }

    protected override int CountActiveObjects()
    {
        int pistolCount = pistolAmmoPool != null ? pistolAmmoPool.CountActiveObjects() : 0;
        int riffleCount = riffleAmmoPool != null ? riffleAmmoPool.CountActiveObjects() : 0;
        int shotgunCount = shotgunAmmoPool != null ? shotgunAmmoPool.CountActiveObjects() : 0;
        int rocketCount = rocketLauncherAmmoPool != null ? rocketLauncherAmmoPool.CountActiveObjects() : 0;

        return pistolCount + riffleCount + shotgunCount + rocketCount;
    }
}