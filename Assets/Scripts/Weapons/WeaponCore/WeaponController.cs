using System;
using System.Linq;

using UnityEngine;

public class WeaponController : MonoBehaviour, IWeapon
{
    private const string LOG_PREFIX = "WeaponController";

    [SerializeField] private WeaponConfigHolder weaponConfigHolder;
    [SerializeField] private BulletsConfig bulletsConfig;

    private WeaponData weaponData;
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
        if (weaponData.CurrentAmmo <= 0)
        {
            weaponData.CurrentAmmo = weaponData.MagazineSize;
        }

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
        return weaponData != null && weaponData.CurrentAmmo > 0;
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

        weaponData.CurrentAmmo--;

        autoReloadHandler?.TryAutoReload();

        Vector3 baseDirection = GetShootDirection();

        if (shotgunBulletController != null)
        {
            shotgunBulletController.Fire(bulletsPool, spawnPoint, weaponData, bulletsData, baseDirection);
        }
        else
        {
            ShootSingle(baseDirection);
        }

        PlayMuzzleFlash();

        OnAmmoChanged?.Invoke();
    }

    private void ShootSingle(Vector3 direction)
    {
        GameObject bullet = bulletsPool.Spawn(spawnPoint.position, spawnPoint.rotation, true);

        if (bullet.TryGetComponent(out IBullet bulletsController))
        {
            bulletsController.Initialize(direction, bulletsPool, weaponData, bulletsData);
        }
        else
        {
            Debug.LogError($"{LOG_PREFIX}: {bullet.name} does not have IBullet component.");
        }
    }

    public WeaponData GetWeaponData()
    {
        return weaponData;
    }

    private Vector3 GetShootDirection()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        return Physics.Raycast(ray, out var hit, weaponData.Range) ? (hit.point - spawnPoint.position).normalized
            : ray.direction;
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
        return weaponData != null ? weaponData.CurrentAmmo : 0;
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
