using System;

using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CollectibleWeapon : CollectibleItems
{
    private const string LOG_PREFIX = "CollectibleWeapon";

    [Header("Weapon pickup data")]
    [SerializeField] private GunsType gunsType = GunsType.Riffle;

    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private bool deactivateOnCollect = true;
    [SerializeField] private WeaponUnlockManager weaponUnlockManager;
    [SerializeField] private string weaponsHolderName = "WeaponsHolder";

    protected override void Start()
    {
        base.Start();
        if (weaponUnlockManager == null)
        {
            weaponUnlockManager = FindObjectOfType<WeaponUnlockManager>();

            //if (weaponUnlockManager != null)
            //{
            //    Debug.Log($"{LOG_PREFIX}: WeaponUnlockManager auto-found.");
            //}
        }
    }

    protected override void Collect(GameObject player)
    {
        if (isCollected)
        {
            return;
        }

        if (weaponUnlockManager != null && !weaponUnlockManager.IsAllowedToSpawn(gunsType))
        {
            Debug.Log($"{LOG_PREFIX}: Collect attempt for {gunsType} denied - spawn not allowed on level {weaponUnlockManager.GetCurrentLevel()}.");
            if (deactivateOnCollect)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        if (weaponPrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX}: weaponPrefab is not assigned on {gameObject.name}. Cannot give weapon.");
            return;
        }

        PlayerInventory playerInventory = null;
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
        }

        bool addedViaInventory = false;

        if (playerInventory != null)
        {
            try
            {
                addedViaInventory = playerInventory.AddWeapon(gunsType, weaponPrefab, true);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{LOG_PREFIX}: Exception while calling PlayerInventory.AddWeapon: {ex.Message}. Falling back to direct instantiate.");
                addedViaInventory = false;
            }

            if (addedViaInventory)
            {
                Debug.Log($"{LOG_PREFIX}: Sent AddWeapon request to PlayerInventory for {gunsType}.");
                isCollected = true;
                if (deactivateOnCollect) gameObject.SetActive(false);
                return;
            }
        }

        GameObject instantiated = null;

        try
        {
            Transform parentTransform = null;
            if (player != null)
            {
                var weaponsHolder = player.transform.Find(weaponsHolderName);
                parentTransform = weaponsHolder != null ? weaponsHolder : player.transform;
            }

            instantiated = Instantiate(weaponPrefab, parentTransform);
            instantiated.name = weaponPrefab.name;

            var rigidbody = instantiated.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
                rigidbody.detectCollisions = false;
            }

            WeaponController weaponController = instantiated.GetComponentInChildren<WeaponController>();
            if (weaponController == null)
            {
                Debug.LogError($"{LOG_PREFIX}: Instantiated prefab does not contain WeaponController. Prefab path: {weaponPrefab.name}");
            }
            else
            {
                PlayerShoot playerShoot = null;
                if (player != null)
                {
                    playerShoot = player.GetComponent<PlayerShoot>();
                }

                if (playerShoot == null)
                {
                    playerShoot = FindObjectOfType<PlayerShoot>();
                }

                if (playerShoot != null)
                {
                    playerShoot.SetCurrentWeapon(weaponController as IWeapon);
                    Debug.Log($"{LOG_PREFIX}: Instantiated and equipped {gunsType} via PlayerShoot.SetCurrentWeapon.");
                }
                else
                {
                    Debug.LogWarning($"{LOG_PREFIX}: PlayerShoot not found. Weapon instantiated but not equipped.");
                }

                isCollected = true;
                if (deactivateOnCollect)
                {
                    gameObject.SetActive(false);
                }

                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"{LOG_PREFIX}: Exception during fallback instantiate: {ex.Message}");
            if (instantiated != null)
            {
                Destroy(instantiated);
            }

            Debug.LogWarning($"{LOG_PREFIX}: Collection for {gunsType} completed with no effect. Deactivating pickup.");
            if (deactivateOnCollect)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
