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

        if (AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType) <= 0)
        {
            Debug.Log("PlayerReload: Нет патронов в запасе");
            return;
        }

        Debug.Log(" PlayerReload. Starting reload...");
        StartCoroutine(ReloadRoutine(weaponData));
    }

    private IEnumerator ReloadRoutine(WeaponData weaponData)
    {
        isReloading = true;

        yield return new WaitForSeconds(2f);

        int ammoNeeded = weaponData.MagazineSize - weaponData.CurrentAmmo;
        int ammoAvailable = AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType);
        int ammoToReload = Mathf.Min(ammoNeeded, ammoAvailable);

        weaponData.CurrentAmmo += ammoToReload;
        AmmoManager.Instance.UseAmmo(weaponData.GunsType, ammoToReload);

        isReloading = false;
        Debug.Log($"PlayerReload: Перезаряжено {ammoToReload} патронов. Остаток: {AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType)}");
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}
