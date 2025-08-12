using System;
using System.Collections;

using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    private PlayerShoot playerShoot;
    private Coroutine reloadCoroutine;

    private bool isReloading = false;

    public event Action OnReloadStarted;
    public event Action OnReloadEnded;

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
    }

    public void Reload()
    {
        if (playerShoot == null || playerShoot.GetCurrentWeapon() == null)
        {
            Debug.LogWarning("PlayerReload: No active weapons!");
            return;
        }

        var weapon = playerShoot.GetCurrentWeapon();
        if (weapon == null)
        {
            Debug.LogWarning("PlayerReload: No current weapon found.");
            return;
        }

        WeaponData weaponData = weapon.GetWeaponData();

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
            Debug.Log("PlayerReload: No ammo in reserve");
            return;
        }

        if (TryGetComponent<PlayerShoot>(out var shoot))
        {
            shoot.StopFiring();
        }

        isReloading = true;
        OnReloadStarted?.Invoke();

        Debug.Log(" PlayerReload. Starting reload...");
        reloadCoroutine = StartCoroutine(ReloadRoutine(weaponData));
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
        //Debug.Log($"PlayerReload: Reload {ammoToReload} ammo. Left: {AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType)}");

        if (playerShoot.GetCurrentWeapon() is WeaponController weaponController)
        {
            weaponController.InvokeAmmoChanged();
        }

        isReloading = false;
        OnReloadEnded?.Invoke();
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public void CancelReload()
    {
        if (!isReloading)
        {
            return;
        }

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        isReloading = false;
        OnReloadEnded?.Invoke();

        Debug.Log("PlayerReload: Reload cancelled due to weapon switch.");
    }
}
