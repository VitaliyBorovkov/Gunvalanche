using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInventory : MonoBehaviour
{
    private const string LOG_PREFIX = "PlayerInventory";

    [Serializable]
    private struct WeaponPrefabEntry
    {
        public GunsType type;
        public GameObject prefab;
    }

    [Serializable]
    private struct WeaponSocketEntry
    {
        public GunsType type;
        public Transform socket;
    }

    [Header("References")]
    [SerializeField] private WeaponUnlockManager weaponUnlockManager;
    [SerializeField] private Transform weaponsHolder;

    [Header("Sockets (children of WeaponsHolder)")]
    [SerializeField] private List<WeaponSocketEntry> weaponSockets = new List<WeaponSocketEntry>();

    [Header("Default prefabs mapping")]
    [SerializeField] private List<WeaponPrefabEntry> defaultWeaponPrefabs = new List<WeaponPrefabEntry>();

    [Header("Startup")]
    [SerializeField] private bool givePistolOnStart = true;

    public event Action<GunsType, WeaponController, bool> OnWeaponAdded;

    private Dictionary<GunsType, WeaponController> ownedWeapons = new Dictionary<GunsType, WeaponController>();
    private Dictionary<GunsType, GameObject> prefabLookup;
    private Dictionary<GunsType, Transform> socketLookup;

    private void Awake()
    {
        if (weaponUnlockManager == null)
        {
            weaponUnlockManager = FindObjectOfType<WeaponUnlockManager>();
        }

        if (weaponsHolder == null)
        {
            weaponsHolder = transform;
        }

        BuildPrefabLookup();
        BuildSocketLookup();
    }

    private void Start()
    {
        if (givePistolOnStart && !HasWeapon(GunsType.Pistol))
        {
            var prefab = GetPrefabFor(GunsType.Pistol);
            if (prefab != null)
            {
                AddWeapon(GunsType.Pistol, prefab, autoEquip: true, ignoreUnlock: true);
            }
            else
            {
                Debug.LogError($"{LOG_PREFIX}: No prefab found for Pistol in defaultWeaponPrefabs.");
            }
        }
    }

    private void BuildPrefabLookup()
    {
        prefabLookup = new Dictionary<GunsType, GameObject>();

        foreach (var entry in defaultWeaponPrefabs)
        {
            if (prefabLookup.ContainsKey(entry.type))
            {
                Debug.LogWarning($"{LOG_PREFIX}: Duplicate prefab entry for {entry.type} found in defaultWeaponPrefabs. Using first occurrence.");
                continue;
            }
            prefabLookup[entry.type] = entry.prefab;
        }
    }

    private void BuildSocketLookup()
    {
        socketLookup = new Dictionary<GunsType, Transform>();
        foreach (var sockets in weaponSockets)
        {
            if (sockets.socket == null)
            {
                Debug.LogWarning($"{LOG_PREFIX}: Socket is null for {sockets.type}.");
                continue;
            }

            if (weaponsHolder != null && sockets.socket != null && sockets.socket.IsChildOf(weaponsHolder) == false)
            {
                Debug.LogWarning($"{LOG_PREFIX}: Socket '{sockets.socket.name}' is not a child of WeaponsHolder. It still will work, but keep hierarchy consistent.");
            }

            if (!socketLookup.ContainsKey(sockets.type))
                socketLookup[sockets.type] = sockets.socket;
        }
    }

    private GameObject GetPrefabFor(GunsType gunsType)
    {
        if (prefabLookup != null && prefabLookup.TryGetValue(gunsType, out var prefab) && prefab != null)
        {
            return prefab;
        }
        return null;
    }

    private Transform GetSocketFor(GunsType type)
    {
        if (socketLookup != null && socketLookup.TryGetValue(type, out var socket) && socket != null)
        {
            return socket;
        }
        return weaponsHolder != null ? weaponsHolder : transform;
    }

    public bool HasWeapon(GunsType gunsType)
    {
        return ownedWeapons.ContainsKey(gunsType);
    }

    public bool AddWeapon(GunsType gunsType, UnityEngine.Object optionalConfig = null, bool autoEquip = true,
        bool ignoreUnlock = false)
    {
        if (!ignoreUnlock && weaponUnlockManager != null && !weaponUnlockManager.IsUnlocked(gunsType))
        {
            Debug.Log($"{LOG_PREFIX}: AddWeapon({gunsType}) denied — not unlocked for current level.");
            return false;
        }

        if (ownedWeapons.ContainsKey(gunsType))
        {
            Debug.Log($"{LOG_PREFIX}: AddWeapon({gunsType}) skipped — already owned.");
            return false;
        }

        GameObject weaponPrefab = optionalConfig as GameObject ?? GetPrefabFor(gunsType);
        if (weaponPrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX}: AddWeapon({gunsType}) failed — prefab not found.");
            return false;
        }

        Transform parentSocket = GetSocketFor(gunsType);
        if (parentSocket != null && parentSocket.localScale != Vector3.one)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Socket '{parentSocket.name}' scale {parentSocket.localScale}. Prefer (1,1,1).");
        }

        GameObject instantiate;
        try
        {
            instantiate = Instantiate(weaponPrefab, parentSocket);
            instantiate.name = weaponPrefab.name;

            instantiate.transform.localPosition = Vector3.zero;
            instantiate.transform.localRotation = Quaternion.identity;
        }
        catch (Exception ex)
        {
            Debug.LogError($"{LOG_PREFIX}: Exception while instantiating prefab for {gunsType}: {ex.Message}");
            return false;
        }

        var rigidbody = instantiate.GetComponentInChildren<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
            rigidbody.detectCollisions = false;
        }

        var weaponController = instantiate.GetComponentInChildren<WeaponController>();
        if (weaponController == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Instantiated prefab does not contain WeaponController. Prefab path: {weaponPrefab.name}");
            Destroy(instantiate);
            return false;
        }

        ownedWeapons[gunsType] = weaponController;
        Debug.Log($"{LOG_PREFIX}: Added weapon {gunsType} (instance: {weaponController.gameObject.name}).");
        OnWeaponAdded?.Invoke(gunsType, weaponController, autoEquip);

        return true;
    }

    public bool TryGetWeaponController(GunsType gunsType, out WeaponController weaponController)
    {
        return ownedWeapons.TryGetValue(gunsType, out weaponController);
    }

    public bool RemoveWepon(GunsType gunsType, bool destroyInstance = true)
    {
        if (!ownedWeapons.TryGetValue(gunsType, out var weaponController))
        {
            return false;
        }

        ownedWeapons.Remove(gunsType);

        if (destroyInstance && weaponController != null)
        {
            Destroy(weaponController.gameObject);
        }

        return true;
    }

    public List<GunsType> GetOwnedWeapons()
    {
        return ownedWeapons.Keys.ToList();
    }
}
