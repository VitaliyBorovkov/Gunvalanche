using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitchWeapon : MonoBehaviour
{
    [Header("WeaponHolder")]
    [SerializeField] private Transform weaponsHolder;

    [Header("Provider")]
    [SerializeField] private MonoBehaviour keyProviderComponent;
    private IWeaponKeyProvider keyProvider;

    [Header("Inventory")]
    [SerializeField] private PlayerInventory playerInventory;

    private PlayerShoot playerShoot;
    private int currentWeaponIndex = 0;

    private List<IWeapon> weaponList = new List<IWeapon>();
    private List<GameObject> weaponGameObjects = new List<GameObject>();

    private const string LOG_PREFIX = "PlayerSwitchWeapon";

    public event Action<string> OnWeaponIconKeyChanged;

    private void Awake()
    {
        if (keyProviderComponent != null)
        {
            keyProvider = keyProviderComponent as IWeaponKeyProvider;
            if (keyProvider == null)
            {
                Debug.LogWarning($"{LOG_PREFIX}: keyProviderComponent is not IWeaponKeyProvider!");
            }
        }

        if (keyProvider == null)
        {
            var found = FindObjectOfType<WeaponKeyProvider>();
            if (found != null)
            {
                keyProvider = found;
                //Debug.Log($"{LOG_PREFIX}: Using WeaponKeyProvider from scene '{found.gameObject.name}'.");
            }
        }
    }

    private void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();

        if (weaponsHolder == null)
        {
            Debug.Log($"{LOG_PREFIX}: WeaponsHolder not assigned!");
            return;
        }

        if (playerInventory == null)
            playerInventory = GetComponent<PlayerInventory>() ?? FindObjectOfType<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.OnWeaponAdded -= HandleWeaponAdded;
            playerInventory.OnWeaponAdded += HandleWeaponAdded;
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}: PlayerInventory not found. Fallback to initial scan only.");
        }

        CollectWeapon();

        if (weaponList.Count > 0)
        {
            SwitchWeaponByIndex(0);
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}:  No weapons have been added to the list.");
        }
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnWeaponAdded -= HandleWeaponAdded;
    }

    private void HandleWeaponAdded(GunsType gunsType, WeaponController weaponController, bool autoEquip)
    {
        if (weaponController == null)
        {
            return;
        }
        var go = weaponController.gameObject;

        if (weaponGameObjects.Contains(go))
        {
            return;
        }

        RegisterWeapon(go, weaponController as IWeapon, autoEquip);
    }

    public void RegisterWeapon(GameObject weaponGO, IWeapon weapon, bool autoEquip = false, string iconKeyOverride = null)
    {
        if (weaponGO == null || weapon == null)
        {
            return;
        }

        bool parentChanged = false;
        if (weaponsHolder != null)
        {
            bool alreadyUnderHolder = weaponGO.transform.IsChildOf(weaponsHolder);
            if (!alreadyUnderHolder)
            {
                weaponGO.transform.SetParent(weaponsHolder, false);
                parentChanged = true;
                Debug.Log($"{LOG_PREFIX}: Re-parented '{weaponGO.name}' under '{weaponsHolder.name}'.");
            }
        }
        else
        {
            Debug.Log($"{LOG_PREFIX}: weaponsHolder is null, keeping current parent for '{weaponGO.name}'.");
        }

        if (parentChanged)
        {
            weaponGO.transform.localPosition = Vector3.zero;
            weaponGO.transform.localRotation = Quaternion.identity;
        }

        weaponGO.SetActive(false);

        weaponList.Add(weapon);
        weaponGameObjects.Add(weaponGO);

        int newIndex = weaponList.Count - 1;
        if (autoEquip || weaponList.Count == 1)
            SwitchWeaponByIndex(newIndex);

        string key = null;
        var identity = weaponGO.GetComponent<WeaponIdentity>();
        if (!string.IsNullOrEmpty(iconKeyOverride))
            key = iconKeyOverride;
        else if (identity != null && !string.IsNullOrEmpty(identity.iconKey))
            key = identity.iconKey;

        OnWeaponIconKeyChanged?.Invoke(key);

        Debug.Log($"{LOG_PREFIX}: Registered weapon '{weaponGO.name}', index={newIndex}, autoEquip={autoEquip}");
    }

    private void CollectWeapon()
    {
        weaponList.Clear();
        weaponGameObjects.Clear();

        var weaponComponents = weaponsHolder.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var monoBehaviour in weaponComponents)
        {
            if (monoBehaviour is IWeapon iWeapon)
            {
                weaponList.Add(iWeapon);
                weaponGameObjects.Add(monoBehaviour.gameObject);
                monoBehaviour.gameObject.SetActive(false);

                //Debug.Log($"{LOG_PREFIX}: Added weapon -> name='{monoBehaviour.gameObject.name}'" +
                //    $" component='{monoBehaviour.GetType().Name}' {info}");
            }
        }
    }

    public void HandleScrollWeapon(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        if (scrollValue > 0)
        {
            SwitchToNextWeapon();
        }
        else if (scrollValue < 0)
        {
            SwitchToPreviousWeapon();
        }
    }

    private void SwitchToNextWeapon()
    {
        if (weaponList.Count == 0)
        {
            return;
        }

        currentWeaponIndex = (currentWeaponIndex + 1) % weaponList.Count;

        SwitchWeaponByIndex(currentWeaponIndex);
    }

    private void SwitchToPreviousWeapon()
    {
        if (weaponList.Count == 0)
        {
            return;
        }

        currentWeaponIndex = (currentWeaponIndex - 1 + weaponList.Count) % weaponList.Count;

        SwitchWeaponByIndex(currentWeaponIndex);
    }

    public void SwitchWeaponByIndex(int index)
    {
        if (weaponList.Count == 0 || index < 0 || index >= weaponList.Count)
        {
            return;
        }

        for (int i = 0; i < weaponList.Count; i++)
        {
            MonoBehaviour weaponMB = weaponList[i] as MonoBehaviour;
            if (weaponMB != null)
            {
                weaponMB.gameObject.SetActive(i == index);
            }
        }

        currentWeaponIndex = index;

        if (playerShoot != null)
        {
            playerShoot.SetCurrentWeapon(weaponList[currentWeaponIndex]);
        }

        string resolvedKey = null;
        var gameObject = (currentWeaponIndex >= 0 && currentWeaponIndex < weaponGameObjects.Count) ?
            weaponGameObjects[currentWeaponIndex] : null;

        if (keyProvider != null && gameObject != null)
        {
            resolvedKey = keyProvider.GetKey(gameObject);
        }

        //Debug.Log($"{LOG_PREFIX}: Resolved icon key='{resolvedKey}' for '{gameObject?.name}'");

        OnWeaponIconKeyChanged?.Invoke(resolvedKey);
    }

    public void SetKeyProvider(IWeaponKeyProvider provider)
    {
        keyProvider = provider;
    }

    public int GetCurrentWeaponIndex() => currentWeaponIndex;
    public GameObject GetCurrentWeaponGameObject() => (currentWeaponIndex >= 0 && currentWeaponIndex <
        weaponGameObjects.Count) ? weaponGameObjects[currentWeaponIndex] : null;
}
