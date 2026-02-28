using System;
using System.Collections;

using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private const string LOG_PREFIX = "PlayerShoot";

    public event Action<IWeapon> OnWeaponChanged;

    private IWeapon currentWeapon;
    private Coroutine shootingCoroutine;
    private HealthController playerHealth;

    private bool isBlockedByReload = false;

    private void Awake()
    {
        if (TryGetComponent<PlayerReload>(out var reload))
        {
            reload.OnReloadStarted += () => isBlockedByReload = true;
            reload.OnReloadEnded += () => isBlockedByReload = false;
        }

        playerHealth = GetComponent<HealthController>();
    }

    public void SetCurrentWeapon(IWeapon weapon)
    {
        if (currentWeapon == weapon)
        {
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
        if (shootingCoroutine != null)
        {
            return;
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            Debug.Log("PlayerShoot: Can't shoot when player is dead.");
            return;
        }

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
        if (shootingCoroutine == null)
        {
            return;
        }

        StopCoroutine(shootingCoroutine);
        shootingCoroutine = null;
    }

    private IEnumerator ShootRoutine()
    {
        var wait = new WaitForSeconds(GetSecondsPerShotSafe());

        while (currentWeapon != null && currentWeapon.CanShoot())
        {
            currentWeapon.Shoot();

            yield return wait;
        }

        StopFiring();
    }

    private float GetSecondsPerShotSafe()
    {
        if (currentWeapon == null)
        {
            return 0.1f;
        }

        var fireRate = currentWeapon.GetFireRate();
        if (fireRate <= 0f)
        {
            return 0.1f;
        }

        return 1f / fireRate;
    }

    public void UpdateWeaponData()
    {
    }
}
