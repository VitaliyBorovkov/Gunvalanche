using System;
using System.Collections;

using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public event Action<IWeapon> OnWeaponChanged;

    private IWeapon currentWeapon;
    private Coroutine shootingCoroutine;

    private bool isBlockedByReload = false;

    private void Awake()
    {
        if (TryGetComponent<PlayerReload>(out var reload))
        {
            reload.OnReloadStarted += () => isBlockedByReload = true;
            reload.OnReloadEnded += () => isBlockedByReload = false;
        }
    }

    public void SetCurrentWeapon(IWeapon weapon)
    {
        if (currentWeapon == weapon)
        {
            Debug.Log("PlayerShoot: Attempted to set the same weapon.");
            return;
        }

        currentWeapon = weapon;

        if (weapon is WeaponController weaponController && TryGetComponent<IAutoReload>(out var autoReload))
        {
            weaponController.SetAutoReloadHandler(autoReload);
        }

        if (TryGetComponent<PlayerReload>(out var reload))
        {
            reload.CancelReload();
        }

        OnWeaponChanged?.Invoke(currentWeapon);
    }

    public IWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public void StartFiring()
    {
        if (isBlockedByReload)
        {
            Debug.Log("PlayerShoot: Can't shoot while reloading.");
            return;
        }

        if (currentWeapon == null || !currentWeapon.CanShoot())
        {
            Debug.Log("PlayerShoot: No ammo or reload processing.");
            return;
        }

        shootingCoroutine = StartCoroutine(ShootRoutine());
    }

    public void StopFiring()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    private IEnumerator ShootRoutine()
    {
        while (currentWeapon != null && currentWeapon.CanShoot())
        {
            currentWeapon.Shoot();

            yield return new WaitForSeconds(1f / currentWeapon.GetFireRate());
        }

        StopFiring();
    }

    public void UpdateWeaponData()
    {
    }
}
