using System.Collections;
using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    private PlayerInventory playerInventory;

    private bool isReloading = false;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
    }

    public void Reload()
    {  
        if (isReloading)
        {
            Debug.Log(" PlayerReload. Already reloading");
            return;
        }
        
        var currentWeapon = playerInventory.GetCurrentWeapon();
        if (!currentWeapon.HasValue)
        {
            Debug.LogWarning(" PlayerReload. No current weapon selected.");
            return;
        }

        WeaponData weaponData = currentWeapon.Value;

        if (weaponData.CurrentAmmo == weaponData.MagazineSize)
        {
            Debug.Log(" PlayerReload. Magazine is full");
            return;
        }

        if (weaponData.TotalAmmo <= 0)
        {
            Debug.Log(" PlayerReload. No ammo left to reload");
            return;
        }

        Debug.Log(" PlayerReload. Starting reload");
        StartCoroutine(ReloadRoutine(weaponData));
    }

    private IEnumerator ReloadRoutine(WeaponData weaponData)
    {
        isReloading = true;
        Debug.Log(" PlayerReload. Reloading...");

        yield return new WaitForSeconds(2f);

        int ammoNeeded = weaponData.MagazineSize - weaponData.CurrentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, weaponData.TotalAmmo);

        weaponData.CurrentAmmo += ammoToReload;
        weaponData.TotalAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log(" PlayerReload. Reloaded: Current Ammo = " + weaponData.CurrentAmmo + ", Total Ammo = " 
            + weaponData.TotalAmmo);
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}
