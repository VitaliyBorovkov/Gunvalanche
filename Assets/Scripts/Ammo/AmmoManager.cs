using System.Collections.Generic;

using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; private set; }

    [SerializeField] private GameObject pistolBulletsPoolPrefab;
    [SerializeField] private GameObject riffleBulletsPoolPrefab;
    [SerializeField] private GameObject shotgunBulletsPoolPrefub;
    [SerializeField] private GameObject rocketsPoolPrefab;

    [SerializeField] private GameObject pistolCasingPoolPrefab;
    [SerializeField] private GameObject riffleCasingPoolPrefab;
    [SerializeField] private GameObject shotgunCasingPoolPrefab;

    private ObjectPool pistolBulletsPool;
    private ObjectPool riffleBulletsPool;
    private ObjectPool shotgunBulletsPool;
    private ObjectPool rocketsPool;

    private ObjectPool pistolCasingPool;
    private ObjectPool riffleCasingPool;
    private ObjectPool shotgunCasingPool;

    private Dictionary<GunsType, int> ammoStorage = new Dictionary<GunsType, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeAmmoStorage();
        InitializePools();
    }

    private void InitializePools()
    {
        pistolBulletsPool = InstantiatePool(pistolBulletsPoolPrefab, "PistolBulletsPool");
        riffleBulletsPool = InstantiatePool(riffleBulletsPoolPrefab, "RiffleBulletsPool");
        shotgunBulletsPool = InstantiatePool(shotgunBulletsPoolPrefub, "ShotgunBulletsPool");
        rocketsPool = InstantiatePool(rocketsPoolPrefab, "RocketsPool");

        pistolCasingPool = InstantiatePool(pistolCasingPoolPrefab, "PistolCasingPool");
        riffleCasingPool = InstantiatePool(riffleCasingPoolPrefab, "RiffleCasingPool");
        shotgunCasingPool = InstantiatePool(shotgunCasingPoolPrefab, "ShotgunCasingPool");
    }

    private ObjectPool InstantiatePool(GameObject poolPrefab, string poolName)
    {
        if (poolPrefab == null)
        {
            Debug.LogWarning($"AmmoManager: Prefab {poolName} not assigned!");
            return null;
        }

        GameObject poolObject = Instantiate(poolPrefab);
        poolObject.name = poolName;

        var pool = poolObject.GetComponent<ObjectPool>();
        if (pool == null)
        {
            Debug.LogError($"AmmoManager: {poolName} prefab has no ObjectPool component!");
        }
        return pool;
    }

    private void InitializeAmmoStorage()
    {
        WeaponConfig[] weaponConfigs = Resources.LoadAll<WeaponConfig>("ScriptableObjects/Weapons");
        if (weaponConfigs.Length == 0)
        {
            return;
        }

        foreach (var config in weaponConfigs)
        {
            foreach (var weapon in config.weaponData)
            {
                if (!ammoStorage.ContainsKey(weapon.GunsType))
                {
                    ammoStorage[weapon.GunsType] = weapon.TotalAmmo;
                }
            }
        }
    }

    public void AddAmmo(GunsType type, int amount, int maxAmmo)
    {
        if (!ammoStorage.ContainsKey(type))
        {
            return;
        }

        ammoStorage[type] = Mathf.Clamp(ammoStorage[type] + amount, 0, maxAmmo);
        Debug.Log($"AmmoManager: Added {amount} ammo for {type}. Total: {ammoStorage[type]}/{maxAmmo}");
    }

    public bool UseAmmo(GunsType type, int amount)
    {
        if (!ammoStorage.ContainsKey(type))
        {
            return false;
        }

        if (ammoStorage[type] >= amount)
        {
            ammoStorage[type] -= amount;
            return true;
        }
        Debug.LogWarning($"AmmoManager: Not enough ammo for {type}! Required: {amount}, available: {ammoStorage[type]}.");
        return false;
    }

    public int GetTotalAmmo(GunsType type)
    {
        if (!ammoStorage.ContainsKey(type))
        {
            return 0;
        }

        return ammoStorage[type];
    }

    public ObjectPool GetBulletsPool(BulletsType bulletsType)
    {
        ObjectPool pool = bulletsType switch
        {
            BulletsType.Pistol => pistolBulletsPool,
            BulletsType.Riffle => riffleBulletsPool,
            BulletsType.Shotgun => shotgunBulletsPool,
            BulletsType.Rocket => rocketsPool,
            _ => null
        };
        return pool;
    }

    public ObjectPool GetCasingPool(BulletsType bulletsType)
    {
        ObjectPool pool = bulletsType switch
        {
            BulletsType.Pistol => pistolCasingPool,
            BulletsType.Riffle => riffleCasingPool,
            BulletsType.Shotgun => shotgunCasingPool,
            _ => null
        };
        return pool;
    }
}
