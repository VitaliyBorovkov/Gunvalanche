using System;
using System.Collections.Generic;

using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; private set; }

    [SerializeField] private GameObject pistolBulletsPoolPrefab;
    [SerializeField] private GameObject riffleBulletsPoolPrefab;
    [SerializeField] private GameObject rocketsPoolPrefab;

    private ObjectPool pistolBulletsPool;
    private ObjectPool riffleBulletsPool;
    private ObjectPool rocketsPool;

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
        rocketsPool = InstantiatePool(rocketsPoolPrefab, "RocketsPool");
    }

    private ObjectPool InstantiatePool(GameObject poolPrefab, string poolName)
    {
        if (poolPrefab == null)
        {
            Debug.LogWarning($"AmmoManager: ������ {poolName} �� ��������!");
            return null;
        }

        GameObject poolObject = Instantiate(poolPrefab);
        poolObject.name = poolName;
        return poolObject.GetComponent<ObjectPool>();
    }

    private void InitializeAmmoStorage()
    {
        WeaponConfig[] weaponConfigs = Resources.LoadAll<WeaponConfig>("ScriptableObjects/Weapons");
        if (weaponConfigs.Length == 0)
        {
            Debug.LogError("AmmoManager: WeaponConfig �� ������! �������, ��� �� ����� � ����� Resources/ScriptableObjects/Weapons.");
            return;
        }

        foreach (var config in weaponConfigs)
        {
            foreach (var weapon in config.weaponData)
            {
                if (!ammoStorage.ContainsKey(weapon.GunsType))
                {
                    ammoStorage[weapon.GunsType] = weapon.TotalAmmo;
                    Debug.Log($"AmmoManager: ����������� {weapon.TotalAmmo} �������� ��� {weapon.GunsType} �� WeaponConfig.");
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
        Debug.Log($"AmmoManager: ��������� {amount} �������� ��� {type}. �����: {ammoStorage[type]}/{maxAmmo}");
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
            Debug.Log($"AmmoManager: ������������ {amount} �������� ��� {type}.");
            return false;
        }
        Debug.LogWarning($"AmmoManager: ������������ �������� ��� {type}! ���������: {amount}, ����: {ammoStorage[type]}.");
        return true;
    }

    public int GetTotalAmmo(GunsType type)
    {
        if (!ammoStorage.ContainsKey(type))
        {
            Debug.LogError($"[AmmoManager] ������! {type} �� ������ � ammoStorage.");
            return 0;
        }

        //int ammo =  ammoStorage.ContainsKey(type) ? ammoStorage[type] : 0;
        //return ammo;
        int totalAmmo = ammoStorage[type];
        Debug.Log($"[AmmoManager] �������� ������� ��� {type}: {totalAmmo}");
        return totalAmmo;
    }

    public ObjectPool GetBulletsPool(BulletsType bulletsType)
    {
        return bulletsType switch
        {
            BulletsType.Pistol => pistolBulletsPool,
            BulletsType.Riffle => riffleBulletsPool,
            BulletsType.Rocket => rocketsPool,
            _ => null
        };
    }
}
