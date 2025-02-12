using System;
using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Transform WeaponsHolder;
    [SerializeField] private PlayerReload playerReload;
    [SerializeField] private BulletsConfig bulletConfig;

    private WeaponData weaponData;
    private BulletsData bulletData;
    private GameObject currentWeapon;
    private Transform cameraTransform;
    private ObjectPool currentBulletsPool;
    private WeaponConfigHolder weaponConfigHolder;

    private bool IsFiring = false;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        if (WeaponsHolder != null && WeaponsHolder.transform.childCount > 0)
        {
            SetCurrentWeapon(WeaponsHolder.transform.GetChild(0).gameObject);
        }

        if (currentWeapon != null)
        {
            SetCurrentWeapon(currentWeapon);
        }
    }

    public void StartFiring()
    {
        if (currentWeapon == null)
        {

            Debug.LogWarning("PlayerShoot: ��� ��������� ������!");
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

    public void SetCurrentWeapon(GameObject weapon)
    {
        currentWeapon = weapon;

        weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();

        if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        {
            Debug.LogError($"PlayerShoot: {weapon.name} �� ����� WeaponConfigHolder!");
            return;
        }

        weaponData = weaponConfigHolder.weaponConfig.weaponData[0];

        currentBulletsPool = AmmoManager.Instance.GetBulletsPool(weaponData.BulletsType);
        if (currentBulletsPool == null)
        {
            Debug.LogError($"PlayerShoot: �� ������ ��� ��� {weaponData.BulletsType}");
        }

        bulletData = GetBulletDataForWeapon(weaponData.BulletsType);

        UpdateWeaponData();
    }

    private BulletsData GetBulletDataForWeapon(BulletsType bulletsType)
    {
        foreach (var bullet in bulletConfig.bulletsData)
        {
            if (bullet.BulletsType == bulletsType)
            {
                return bullet;
            }
        }
        Debug.LogWarning($"PlayerShoot: �� ������ BulletsData ��� {bulletsType}");
        return new BulletsData();
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
        if (currentWeapon == null || currentBulletsPool == null)
        {

            Debug.LogWarning("PlayerShoot: ��� ��������� ������ ��� ��� �� ��������!");
            return;
        }

        weaponData = GetWeaponData();
        if (weaponData == null)
        {
            Debug.LogWarning("PlayerShoot: ������ ��������� ������ ������!");
            return;
        }

        weaponData.CurrentAmmo--;

        Transform spawnPoint = currentWeapon.GetComponent<WeaponConfigHolder>().bulletSpawnPoint;
        if (spawnPoint == null)
        {
            Debug.LogWarning("PlayerShoot: Bullet Spawn Point �� ��������!");
            return;
        }


        Debug.Log($"PlayerShoot: �������� {weaponData.Name}, ���������� ��� {currentBulletsPool.gameObject.name}");

        GameObject bullet = currentBulletsPool.Spawn(spawnPoint.position, spawnPoint.rotation);
        bullet.transform.forward = spawnPoint.forward;
        if (bullet == null)
        {
            Debug.LogWarning("PlayerShoot: Object Pool ������ null ��� ������� ������!");
            return;
        }

        BulletsController bulletsController = bullet.GetComponent<BulletsController>();
        if (bulletsController == null)
        {
            Debug.LogWarning("PlayerShoot: � ������� ���� ��� BulletsController!");
            return;
        }

        //bulletsController.SetPool(currentBulletsPool);


        //float range = weaponData.Range;
        //Vector3 target = cameraTransform.position + cameraTransform.forward * range;
        //RaycastHit hit;
        //if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
        //{
        //    target = hit.point;

        //    if (hit.collider.CompareTag("Enemy"))
        //    {
        //        HealthController healthController = hit.collider.GetComponentInParent<HealthController>();
        //        if (healthController != null)
        //        {
        //            healthController.TakeDamage(weaponData.Damage);
        //        }
        //    }
        //}
        //bulletsController.bulletData.Target = target;
        //bulletsController.bulletData.HitTarget = true;
        //bullet.transform.forward = (target - bullet.transform.position).normalized;

        //StartCoroutine(DespawnBulletAfterTime(bullet, bulletData.LifeTime));
        Vector3 shootDirection = cameraTransform.forward;
        if (currentBulletsPool == null)
        {
            Debug.LogError($"PlayerShoot: Object Pool �� ������ ��� {weaponData.BulletsType}");
            return;
        }
        bulletsController.Initialize(shootDirection, currentBulletsPool, weaponData);
    }

    //private IEnumerator DespawnBulletAfterTime(GameObject bullet, float time)
    //{
    //    yield return new WaitForSeconds(time);

    //    if (currentBulletsPool != null)
    //    {
    //        Debug.Log($"PlayerShoot: ��������� {bullet.name} ����� ObjectPool {currentBulletsPool.gameObject.name}");
    //        currentBulletsPool.Despawn(bullet);
    //    }
    //    else
    //    {
    //        Debug.LogError("PlayerShoot: Object Pool �� ������ ��� ������� ������� ����!");
    //    }
    //}
}
