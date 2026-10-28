using System;
using System.Collections;

using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    private const string LOG_PREFIX = "PlayerReload";

    [SerializeField] private float reloadDuration = 2f;

    private PlayerShoot playerShoot;
    private HealthController playerHealth;
    private Coroutine reloadCoroutine;

    private bool isReloading = false;

    public event Action OnReloadStarted;
    public event Action OnReloadEnded;

    public float ReloadDuration => reloadDuration;

    // How many rounds this reload will actually add — computed up front (before the wait) so
    // per-shot reload animations (e.g. the shotgun loading shells one at a time) know how many
    // cycles to play instead of guessing a fixed count.
    public int PendingReloadAmount { get; private set; }

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
        playerHealth = GetComponent<HealthController>();
    }

    public void Reload()
    {
        if (playerShoot == null || playerShoot.GetCurrentWeapon() == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: No active weapons!");
            return;
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            Debug.Log($"{LOG_PREFIX}: Can't reload when player is dead.");
            return;
        }

        var weapon = playerShoot.GetCurrentWeapon();
        WeaponData weaponData = weapon.GetWeaponData();

        if (isReloading)
        {
            Debug.Log($"{LOG_PREFIX}: Already reloading.");
            return;
        }

        if (weapon.GetCurrentAmmoInClip() == weaponData.MagazineSize)
        {
            Debug.Log($"{LOG_PREFIX}: Magazine is full.");
            return;
        }

        if (AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType) <= 0)
        {
            Debug.Log($"{LOG_PREFIX}: No ammo in reserve.");
            return;
        }

        playerShoot.StopFiring();

        int ammoNeeded = weaponData.MagazineSize - weapon.GetCurrentAmmoInClip();
        int ammoAvailable = AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType);
        PendingReloadAmount = Mathf.Min(ammoNeeded, ammoAvailable);

        isReloading = true;
        OnReloadStarted?.Invoke();

        Debug.Log($"{LOG_PREFIX}: Starting reload...");
        reloadCoroutine = StartCoroutine(ReloadRoutine(weapon, weaponData));
    }

    private IEnumerator ReloadRoutine(IWeapon weapon, WeaponData weaponData)
    {
        yield return new WaitForSeconds(reloadDuration);

        weapon.AddAmmoToMagazine(PendingReloadAmount);
        AmmoManager.Instance.UseAmmo(weaponData.GunsType, PendingReloadAmount);

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

        Debug.Log($"{LOG_PREFIX}: Reload cancelled due to weapon switch.");
    }
}
