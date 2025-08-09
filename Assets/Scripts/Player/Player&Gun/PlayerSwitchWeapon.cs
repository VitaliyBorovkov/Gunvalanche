using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitchWeapon : MonoBehaviour
{
    public Transform weaponsHolder;

    private List<IWeapon> weaponList = new List<IWeapon>();
    private int currentWeaponIndex = 0;
    private PlayerShoot playerShoot;

    private void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();

        if (weaponsHolder == null)
        {
            Debug.Log("PlayerSwitchWeapon: WeaponsHolder not assigned!");
            return;
        }

        foreach (Transform weaponTransform in weaponsHolder)
        {
            GameObject weaponGO = weaponTransform.gameObject;
            IWeapon weapon = weaponGO.GetComponent<IWeapon>();
            if (weapon != null)
            {
                weaponList.Add(weapon);
                weaponGO.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"PlayerSwitchWeapon: {weaponGO.name} does not implement IWeapon interface!");
            }
        }

        if (weaponList.Count > 0)
        {
            SwitchWeaponByIndex(0);
        }
        else
        {
            Debug.LogWarning("PlayerSwitchWeapon:  No weapons have been added to the list.");
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
    }
}
