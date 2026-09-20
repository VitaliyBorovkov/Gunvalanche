using System;
using System.Linq;

using UnityEngine;

public class WeaponController : MonoBehaviour, IWeapon
{
    private const string LOG_PREFIX = "WeaponController";
    private const float MaxAimCorrectionAngle = 60f;

    [SerializeField] private WeaponConfigHolder weaponConfigHolder;
    [SerializeField] private BulletsConfig bulletsConfig;

    private WeaponData weaponData;
    private WeaponRuntimeData runtimeData;
    private BulletsData bulletsData;
    private ObjectPool bulletsPool;
    private Transform spawnPoint;
    private IAutoReload autoReloadHandler;

    private ShotgunBulletController shotgunBulletController;

    public event Action OnAmmoChanged;

    private void Awake()
    {
        shotgunBulletController = GetComponent<ShotgunBulletController>();

        if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        {
            Debug.LogError($"{LOG_PREFIX}: weaponConfigHolder or weaponConfig is not set on {gameObject.name}.");
            return;
        }

        weaponData = weaponConfigHolder.weaponConfig.weaponData[0];

        // Ammo is per-instance runtime state, never written back onto weaponData/the
        // ScriptableObject asset — see WeaponRuntimeData and DECISIONS.md.
        int initialAmmo = weaponData.CurrentAmmo > 0 ? weaponData.CurrentAmmo : weaponData.MagazineSize;
        runtimeData = new WeaponRuntimeData(initialAmmo);

        spawnPoint = weaponConfigHolder.bulletSpawnPoint;
        if (spawnPoint == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Bullet spawn point is not set on {gameObject.name}.");
        }
    }

    private void Start()
    {
        if (bulletsConfig != null)
        {
            bulletsData = bulletsConfig.bulletsData.FirstOrDefault(b => b.BulletsType == weaponData.BulletsType);
        }

        if (AmmoManager.Instance != null)
        {
            bulletsPool = AmmoManager.Instance.GetBulletsPool(weaponData.BulletsType);
        }

        if (bulletsPool == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Bullets pool is null for BulletsType={weaponData.BulletsType}.");
        }
    }

    public bool CanShoot()
    {
        return weaponData != null && runtimeData != null && runtimeData.CurrentAmmo > 0;
    }

    public float GetFireRate()
    {
        return weaponData != null ? weaponData.FireRate : 0f;
    }

    public void Shoot()
    {
        if (!CanShoot() || bulletsPool == null || spawnPoint == null)
        {
            return;
        }

        runtimeData.Decrement();

        autoReloadHandler?.TryAutoReload();

        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        bool hasAimHit = Physics.Raycast(ray, out var aimHit, weaponData.Range);

        // Past this distance the projectile's own clamped direction (see GetShootDirection)
        // reliably converges close enough to hit a normal-sized target on its own. Closer
        // than the muzzle's own offset from the camera, no forward-ish direction fired from
        // the muzzle can geometrically reach a point that's effectively behind it — resolve
        // the shot immediately at the aim raycast's own (unoffset, always exact) hit point
        // instead of trusting the fired projectile to physically reach it.
        bool isPointBlank = hasAimHit
            && aimHit.distance <= Vector3.Distance(ray.origin, spawnPoint.position);
        RaycastHit? pointBlankHit = isPointBlank ? aimHit : (RaycastHit?)null;

        Vector3 baseDirection = GetShootDirection(ray, hasAimHit, aimHit);

        if (shotgunBulletController != null)
        {
            shotgunBulletController.Fire(bulletsPool, spawnPoint, weaponData, bulletsData, baseDirection,
                pointBlankHit);
        }
        else
        {
            ShootSingle(baseDirection, pointBlankHit);
        }

        PlayMuzzleFlash();

        OnAmmoChanged?.Invoke();
    }

    private void ShootSingle(Vector3 direction, RaycastHit? pointBlankHit)
    {
        GameObject bullet = bulletsPool.Spawn(spawnPoint.position, spawnPoint.rotation, true);

        if (!bullet.TryGetComponent(out IBullet bulletsController))
        {
            Debug.LogError($"{LOG_PREFIX}: {bullet.name} does not have IBullet component.");
            return;
        }

        if (pointBlankHit.HasValue)
        {
            bulletsController.ResolveImmediately(bulletsPool, weaponData,
                pointBlankHit.Value.point, pointBlankHit.Value.collider);
        }
        else
        {
            bulletsController.Initialize(direction, bulletsPool, weaponData, bulletsData);
        }
    }

    public WeaponData GetWeaponData()
    {
        return weaponData;
    }

    private Vector3 GetShootDirection(Ray ray, bool hasHit, RaycastHit hit)
    {
        Vector3 rawDirection = hasHit
            ? (hit.point - spawnPoint.position).normalized
            : ray.direction;

        // The muzzle sits offset from the camera (viewmodel), so converging exactly on the
        // aimed-at point needs a sharper turn the closer the target is. Past some point that
        // turn becomes absurd (firing sideways or even back towards the player) — clamp how
        // far the shot is allowed to deviate from where the player is actually looking.
        // Deliberately clamped against the CAMERA's own forward, not spawnPoint.forward:
        // WeaponSway continuously rotates the weapon's visual transform based on smoothed
        // mouse input, independently of the camera, so spawnPoint.forward can lag well
        // behind the camera's real aim after a quick turn — clamping against it would let
        // the shot fly toward that stale direction instead of towards the target.
        // Normal-range shots need only a few degrees of correction and are unaffected; only
        // extreme point-blank targets get capped — and those are resolved immediately instead
        // of relying on this direction at all, see Shoot()/ResolveImmediately.
        return Vector3.RotateTowards(ray.direction, rawDirection,
            MaxAimCorrectionAngle * Mathf.Deg2Rad, 0f);
    }

    private void PlayMuzzleFlash()
    {
        if (weaponData.MuzzleFlashPrefab == null || spawnPoint == null)
        {
            return;
        }

        var flash = Instantiate(weaponData.MuzzleFlashPrefab, spawnPoint.position, spawnPoint.rotation);
        if (flash.TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Play();
            Destroy(flash, particleSystem.main.duration);
        }
        else
        {
            Destroy(flash, 1f);
        }
    }

    public int GetCurrentAmmoInClip()
    {
        return runtimeData != null ? runtimeData.CurrentAmmo : 0;
    }

    public void AddAmmoToMagazine(int amount)
    {
        if (runtimeData == null || weaponData == null)
        {
            return;
        }

        runtimeData.Add(amount, weaponData.MagazineSize);
    }

    public int GetTotalAmmo()
    {
        if (weaponData == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: weaponData is null in GetTotalAmmo()");
            return 0;
        }

        return AmmoManager.Instance.GetTotalAmmo(weaponData.GunsType);
    }

    public void InvokeAmmoChanged()
    {
        OnAmmoChanged?.Invoke();
    }

    public void SetAutoReloadHandler(IAutoReload autoReload)
    {
        autoReloadHandler = autoReload;
    }
}
