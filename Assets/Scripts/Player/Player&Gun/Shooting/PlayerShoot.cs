using System;
using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private PlayerReload playerReload;
    [SerializeField] private ObjectPool bulletsPool;
    [SerializeField] private BulletsData bulletData;
    [SerializeField] private Transform WeaponsHolder;


    private bool IsFiring = false;
    private Transform cameraTransform;
    private GameObject currentWeapon;
    private WeaponConfigHolder weaponConfigHolder;
    private WeaponData weaponData;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;

        if (bulletsPool == null)
        {
            bulletsPool = FindObjectOfType<ObjectPool>();
            if (bulletsPool == null)
            {
                Debug.LogError("PlayerShoot: ObjectPool не найден на сцене!");
            }
        }
    }

    private void Start()
    {
        if (WeaponsHolder != null && WeaponsHolder.transform.childCount > 0)
        {
            SetCurrentWeapon(WeaponsHolder.transform.GetChild(0).gameObject);
        }
    }

    public void StartFiring()
    {
        if (currentWeapon == null)
        {
            Debug.Log("PlayerShoot: Нет активного оружия!");
            return;
        }

        weaponData = GetWeaponData();
        if (weaponData.CurrentAmmo > 0 && !playerReload.IsReloading())
        {
            IsFiring = true;
            StartCoroutine(ShootAuto());
        }
    }

    public void StopFiring()
    {
        IsFiring = false;
        StopAllCoroutines();
    }

    private IEnumerator ShootAuto()
    {
        weaponData = GetWeaponData();

        while (IsFiring && weaponData.CurrentAmmo > 0)
        {
            ShootGun();
            yield return new WaitForSeconds(1f / weaponData.FireRate);
        }

        StopFiring();
    }

    public void SetObjectPool(ObjectPool pool)
    {
        bulletsPool = pool;
    }

    public void SetCurrentWeapon(GameObject weapon)
    {
        currentWeapon = weapon;

        weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();
        //if (weaponConfigHolder != null)
        //{
        //    Debug.Log($"PlayerShoot: Оружие сменилось на {weapon.name}, обновляем ObjectPool!");
        //    bulletsPool = weaponConfigHolder.weaponConfig.weaponData[0].GunPrefab.GetComponent<ObjectPool>();
        //}
        //else
        //{
        //    Debug.LogError("PlayerShoot: WeaponConfigHolder не найден!");
        //}

        if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        {
            Debug.LogError($"PlayerShoot: {weapon.name} не имеет WeaponConfigHolder!");
            return;
        }

        if (weaponConfigHolder.weaponConfig.weaponData[0].GunPrefab.TryGetComponent(out ObjectPool newPool))
        {
            bulletsPool = newPool;
        }

        UpdateWeaponData();
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponData GetWeaponData()
    {
        if (currentWeapon == null)
        {
            return new WeaponData();
        }

        weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();
        if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        {
            return new WeaponData();
        }

        return weaponConfigHolder.weaponConfig.weaponData[0];
    }

    public void UpdateWeaponData()
    {
        if (currentWeapon == null)
        {
            return;
        }

        weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();
        if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        {
            return;
        }

        weaponData = weaponConfigHolder.weaponConfig.weaponData[0];
    }

    public void ShootGun()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("PlayerShoot: Нет активного оружия!");
            return;
        }

        weaponData = GetWeaponData();
        if (weaponData == null)
        {
            Debug.LogWarning("PlayerShoot: Ошибка получения данных оружия!");
            return;
        }

        Transform spawnPoint = currentWeapon.GetComponent<WeaponConfigHolder>().bulletSpawnPoint;
        if (spawnPoint == null)
        {
            Debug.LogWarning("PlayerShoot: Bullet Spawn Point не назначен!");
            return;
        }

        if (bulletsPool == null)
        {
            Debug.LogWarning("PlayerShoot: Object Pool не назначен! Невозможно создать пулю.");
            return;
        }

        GameObject bullet = bulletsPool.Spawn(spawnPoint.position, Quaternion.identity);
        if (bullet == null)
        {
            Debug.LogWarning("PlayerShoot: Object Pool вернул null при попытке спавна!");
            return;
        }

        BulletsController bulletsController = bullet.GetComponent<BulletsController>();
        if (bulletsController == null)
        {
            Debug.LogWarning("PlayerShoot: У объекта пули нет BulletsController!");
            return;
        }

        float range = weaponData.Range;
        bool hitTarget = false;
        Vector3 target = cameraTransform.position + cameraTransform.forward * range;

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
        {
            target = hit.point;
            hitTarget = true;

            if (hit.collider.CompareTag("Enemy"))
            {
                HealthController healthController = hit.collider.GetComponentInParent<HealthController>();
                if (healthController != null)
                {
                    healthController.TakeDamage(weaponData.Damage);
                }
            }
        }
        bulletsController.bulletData.Target = target;
        bulletsController.bulletData.HitTarget = hitTarget;
        bullet.transform.forward = target - bullet.transform.position;

        StartCoroutine(DespawnBulletAfterTime(bullet, bulletData.LifeTime));
        weaponData.CurrentAmmo--;
    }

    private IEnumerator DespawnBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);

        if (bulletsPool != null)
        {
            bulletsPool.Despawn(bullet);
        }
        else
        {
            Debug.LogError("PlayerShoot: Object Pool не найден при попытке удалить пулю!");
        }
    }
}
