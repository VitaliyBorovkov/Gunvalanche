using System.Linq;

using UnityEngine;

public class WeaponController : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponConfigHolder weaponConfigHolder;
    [SerializeField] private BulletsConfig bulletsConfig;

    private WeaponData weaponData;
    private BulletsData bulletsData;
    private ObjectPool bulletsPool;
    private Transform spawnPoint;

    private void Start()
    {
        if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        {
            Debug.LogError($"WeaponController: weaponConfigHolder or weaponConfig is not set on {gameObject.name}.");
            return;
        }

        weaponData = weaponConfigHolder.weaponConfig.weaponData[0];
        if (weaponData.CurrentAmmo <= 0)
        {
            weaponData.CurrentAmmo = weaponData.MagazineSize;
        }

        bulletsData = bulletsConfig.bulletsData.FirstOrDefault(b => b.BulletsType == weaponData.BulletsType);
        bulletsPool = AmmoManager.Instance.GetBulletsPool(weaponData.BulletsType);
        spawnPoint = weaponConfigHolder.bulletSpawnPoint;
    }

    public bool CanShoot()
    {
        return weaponData.CurrentAmmo > 0;
    }

    public float GetFireRate()
    {
        return weaponData.FireRate;
    }

    public void Shoot()
    {
        if (!CanShoot() || bulletsPool == null || spawnPoint == null)
        {
            return;
        }

        weaponData.CurrentAmmo--;

        GameObject bullet = bulletsPool.Spawn(spawnPoint.position, spawnPoint.rotation, true);

        if (bullet.TryGetComponent(out IBullet bulletsController))
        {
            Vector3 direction = GetShootDirection();
            bulletsController.Initialize(direction, bulletsPool, weaponData, bulletsData);
        }
        else
        {
            Debug.LogError($"WeaponController: {bullet.name} does not have BaseBulletController!");
        }

        PlayMuzzleFlash();
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
}
