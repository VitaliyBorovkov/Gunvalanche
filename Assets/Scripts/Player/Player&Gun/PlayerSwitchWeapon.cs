using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitchWeapon : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private WeaponIconManager weaponIconManager;

    [Header("Weapon")]
    [SerializeField] private Transform weaponsHolder;

    private PlayerShoot playerShoot;
    private int currentWeaponIndex = 0;

    private List<IWeapon> weaponList = new List<IWeapon>();
    private List<GameObject> weaponGameObjects = new List<GameObject>();

    private const string LOG_PREFIX = "PlayerSwitchWeapon";

    private void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();

        if (weaponIconManager == null)
        {
            var found = FindObjectOfType<WeaponIconManager>();
            if (found != null)
            {
                weaponIconManager = found;
            }
        }

        if (weaponsHolder == null)
        {
            Debug.Log($"{LOG_PREFIX}: WeaponsHolder not assigned!");
            return;
        }

        //foreach (Transform weaponTransform in weaponsHolder)
        //{
        //    GameObject weaponGO = weaponTransform.gameObject;
        //    IWeapon weapon = weaponGO.GetComponent<IWeapon>();
        //    if (weapon != null)
        //    {
        //        weaponList.Add(weapon);
        //        weaponGO.SetActive(false);
        //    }
        //    else
        //    {
        //        Debug.LogWarning($"{LOG_PREFIX}: {weaponGO.name} does not implement IWeapon interface!");
        //    }
        //}
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

                Debug.Log($"{LOG_PREFIX}: Added weapon -> name='{monoBehaviour.gameObject.name}'" +
                    $" component='{monoBehaviour.GetType().Name}' {info}");
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

        string iconKey = GetIconKeyFromWeapon(weaponList[currentWeaponIndex]);
        if (weaponIconManager != null)
        {
            if (!string.IsNullOrEmpty(iconKey))
            {
                weaponIconManager.ShowIconForKey(iconKey);
                Debug.Log($"{LOG_PREFIX}: Requested icon for '{iconKey}'.");
            }
            else
            {
                weaponIconManager.ClearIcon();
                Debug.Log($"{LOG_PREFIX}: No key found - cleared icon.");
            }
        }
        else
        {
            Debug.Log($"{LOG_PREFIX}: weaponIconManager not assigned (no UI update).");
        }
    }

    private string GetIconKeyFromWeapon(IWeapon weapon)
    {
        if (weapon == null)
        {
            return null;
        }

        var monoBehaviour = weapon as MonoBehaviour;
        if (monoBehaviour == null)
        {
            return null;
        }

        var identity = monoBehaviour.GetComponent<WeaponIdentity>();
        if (identity != null && !string.IsNullOrEmpty(identity.iconKey))
        {
            return identity.iconKey.Trim();
        }

        string name = monoBehaviour.gameObject.name;
        name = name.Replace("(Clone)", "").Trim();
        return string.IsNullOrEmpty(name) ? null : name;
    }

    public void SetWeaponIconManager(WeaponIconManager iconManager)
    {
        weaponIconManager = iconManager;
        Debug.Log($"{LOG_PREFIX}: WeaponIconManager assigned via SetWeaponIconManager -> " +
            $"{(iconManager != null ? iconManager.name : "null")}");
    }
}
