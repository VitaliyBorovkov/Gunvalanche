using System.Collections;
using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    private PlayerShoot playerShoot;
    private bool isReloading = false;

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
    }

    public void Reload()
    {
        if (playerShoot == null || playerShoot.GetCurrentWeapon() == null)
        {
            Debug.LogWarning("PlayerReload: Нет активного оружия!");
            return;
        }

        //WeaponConfigHolder weaponConfigHolder = playerShoot.GetCurrentWeapon().GetComponent<WeaponConfigHolder>();
        //if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        //{
        //    Debug.LogWarning("PlayerReload: Не найден WeaponConfigHolder или WeaponConfig!");
        //    return;
        //}

        //WeaponData weaponData = weaponConfigHolder.weaponConfig.weaponData[0];
        WeaponData weaponData = playerShoot.GetWeaponData();

        if (isReloading)
        {
            Debug.Log(" PlayerReload. Already reloading");
            return;
        }
       
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
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log(" PlayerReload. Reloading...");

        yield return new WaitForSeconds(2f);

        WeaponData weaponData = playerShoot.GetWeaponData();

        int ammoNeeded = weaponData.MagazineSize - weaponData.CurrentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, weaponData.TotalAmmo);

        weaponData.CurrentAmmo += ammoToReload;
        weaponData.TotalAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log(" PlayerReload. Reloaded: Current Ammo = " + weaponData.CurrentAmmo + ", Total Ammo = " + weaponData.TotalAmmo);
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}
