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
        if (poolPrefab = null)
        {
            Debug.LogWarning($"AmmoManager: Префаб {poolName} не назначен!");
            return null;
        }

        GameObject poolObject = Instantiate(poolPrefab);
        poolObject.name = poolName;
        return poolObject.GetComponent<ObjectPool>();
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
