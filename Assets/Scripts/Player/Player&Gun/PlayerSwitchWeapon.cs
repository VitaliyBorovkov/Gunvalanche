using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerSwitchWeapon : MonoBehaviour
{
    [Header("Weapon Config")]

    public Transform weaponsHolder;

    private List<GameObject> weaponInstances = new List<GameObject>();
    private WeaponConfig currentWeaponConfig;
    private int currentWeaponIndex = 0;
    private PlayerShoot playerShoot;

    private void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();

        if (weaponsHolder == null)
        {
            Debug.Log("PlayerSwitchWeapon: WeaponsHolder не назначен!");
            return;
        }

        foreach (Transform weapon in weaponsHolder)
        {
            weaponInstances.Add(weapon.gameObject);
            weapon.gameObject.SetActive(false);
        }

        if (weaponInstances.Count > 0)
        {
            SwitchWeaponByIndex(0);
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
        if (weaponInstances.Count == 0)
        {
            return;
        }

        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponInstances.Count)
        {
            currentWeaponIndex = 0;
        }

        SwitchWeaponByIndex(currentWeaponIndex);
    }

    private void SwitchToPreviousWeapon()
    {
        if (weaponInstances.Count == 0)
        {
            return;
        }

        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weaponInstances.Count - 1;
        }

        SwitchWeaponByIndex(currentWeaponIndex);
    }

    public void SwitchWeaponByIndex(int index)
    {
        if (weaponInstances.Count == 0 || index < 0 || index >= weaponInstances.Count)
        {
            return;
        }

        foreach (var weapon in weaponInstances)
        {
            weapon.SetActive(false);
        }

        currentWeaponIndex = index;
        weaponInstances[currentWeaponIndex].SetActive(true);


        WeaponConfigHolder  holder = weaponInstances[currentWeaponIndex].GetComponent<WeaponConfigHolder>();
        if (holder != null && holder.weaponConfig != null)
        {
            currentWeaponConfig = holder.weaponConfig;
        
            if (playerShoot != null)
            {
                playerShoot.SetCurrentWeapon(weaponInstances[currentWeaponIndex]);
                playerShoot.UpdateWeaponData();
            }
        }
        else
        {
            Debug.LogWarning($"PlayerSwitchWeapon: У {weaponInstances[currentWeaponIndex].name} нет WeaponConfigHolder или WeaponConfig!");
        }
    }
}
