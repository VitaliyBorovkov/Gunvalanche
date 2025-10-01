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

    protected override void Collect(GameObject player)
    {
        if (isCollected)
        {
            return;
        }

        if (weaponUnlockManager != null && !weaponUnlockManager.IsAllowedToSpawn(gunsType))
        {
            Debug.Log($"{LOG_PREFIX}: {name}: Spawn denied for {gunsType} on level {weaponUnlockManager.GetCurrentLevel()}.");
            return;
        }

        if (weaponPrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX}: weaponPrefab is not assigned on {gameObject.name}. Cannot give weapon.");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: {name}: player is null in Collect().");
            return;
        }

        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: {name}: PlayerInventory not found on player.");
            return;
        }

        bool added = inventory.AddWeapon(gunsType, weaponPrefab, autoEquip: true);
        if (!added)
        {
            Debug.Log($"{LOG_PREFIX}: {name}: AddWeapon returned false (already owned or locked).");
            return;
        }

        isCollected = true;
        if (deactivateOnCollect)
        {
            gameObject.SetActive(false);
        }

        Debug.Log($"{LOG_PREFIX}: {name}: Picked up {gunsType}.");
    }
}
