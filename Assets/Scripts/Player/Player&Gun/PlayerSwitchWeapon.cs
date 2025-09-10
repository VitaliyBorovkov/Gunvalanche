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

                var identity = monoBehaviour.GetComponent<WeaponIdentity>();
                string info = identity != null && !string.IsNullOrEmpty(identity.iconKey)
                    ? $"(identity='{identity.iconKey}')" : "(no identity)";

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
        //Debug.Log($"{LOG_PREFIX}: Key provider set -> " +
        //    $"{(provider is MonoBehaviour mb ? mb.name : provider?.GetType().Name)}");
    }

    public int GetCurrentWeaponIndex() => currentWeaponIndex;
    public GameObject GetCurrentWeaponGameObject() => (currentWeaponIndex >= 0 && currentWeaponIndex <
        weaponGameObjects.Count) ? weaponGameObjects[currentWeaponIndex] : null;
}
