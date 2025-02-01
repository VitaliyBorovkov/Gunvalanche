using System;
using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Transform WeaponsHolder;
    [SerializeField] private PlayerReload playerReload;
    [SerializeField] private BulletsConfig bulletConfig;
    //[SerializeField] private ObjectPool bulletsPool;

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
        
        //if (bulletsPool == null)
        //{
        //    bulletsPool = FindObjectOfType<ObjectPool>();
        //    if (bulletsPool == null)
        //    {
        //        Debug.LogError("PlayerShoot: ObjectPool �� ������ �� �����!");
        //    }
        //}

        if (bulletsPool == null)
        {
            bulletsPool = FindObjectOfType<ObjectPool>();
            if (bulletsPool == null)
            {
                Debug.LogError("PlayerShoot: ObjectPool �� ������ �� �����!");
            }
        }
    }

    private void Start()
    {
        //if (WeaponsHolder != null && WeaponsHolder.transform.childCount > 0)
        //{
        //    SetCurrentWeapon(WeaponsHolder.transform.GetChild(0).gameObject);
        //}
        if (currentWeapon != null)
        {
            SetCurrentWeapon(currentWeapon);
        }
    }

    public void StartFiring()
    {
        if (currentWeapon == null)
        {
            Debug.Log("PlayerShoot: ��� ��������� ������!");
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

    //public void SetObjectPool(ObjectPool pool)
    //{
    //    bulletsPool = pool;
    //}

    public void SetCurrentWeapon(GameObject weapon)
    {
        currentWeapon = weapon;

        weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();
        //if (weaponConfigHolder != null)
        //{
        //    Debug.Log($"PlayerShoot: ������ ��������� �� {weapon.name}, ��������� ObjectPool!");
        //    bulletsPool = weaponConfigHolder.weaponConfig.weaponData[0].GunPrefab.GetComponent<ObjectPool>();
        //}
        //else
        //{
        //    Debug.LogError("PlayerShoot: WeaponConfigHolder �� ������!");
        //}

        if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        {
            Debug.LogError($"PlayerShoot: {weapon.name} �� ����� WeaponConfigHolder!");
            return;
        }

        weaponData = weaponConfigHolder.weaponConfig.weaponData[0];

        currentBulletsPool = AmmoManager.Instance.GetBulletsPool(weaponData.BulletsType);

        bulletData = GetBulletDataForWeapon(weaponData.BulletsType);

        //if (weaponConfigHolder.weaponConfig.weaponData[0].GunPrefab.TryGetComponent(out ObjectPool newPool))
        //{
        //    bulletsPool = newPool;
        //}

        UpdateWeaponData();
    }

    private BulletsData GetBulletDataForWeapon(BulletsType bulletsType)
    {
        foreach(var bullet in bulletConfig.bulletsData)
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


        //if (bulletsPool == null)
        //{
        //    Debug.LogWarning("PlayerShoot: Object Pool �� ��������! ���������� ������� ����.");
        //    return;
        //}

        GameObject bullet = currentBulletsPool.Spawn(spawnPoint.position, Quaternion.identity);
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

        float range = weaponData.Range;
        //bool hitTarget = false;
        Vector3 target = cameraTransform.position + cameraTransform.forward * range;

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
        {
            target = hit.point;
            //hitTarget = true;

            if (hit.collider.CompareTag("Enemy"))
            {
                HealthController healthController = hit.collider.GetComponentInParent<HealthController>();
                if (healthController != null)
                {
                    healthController.TakeDamage(weaponData.Damage);
                }

        if (bulletsPool == null)
        {
            Debug.LogWarning("PlayerShoot: Object Pool �� ��������! ���������� ������� ����.");
            return;
        }



            }
        }
        bulletsController.bulletData.Target = target;
        bulletsController.bulletData.HitTarget = /*hitTarget*/true;
        bullet.transform.forward = target - bullet.transform.position;

        StartCoroutine(DespawnBulletAfterTime(bullet, weaponData.Range / bulletData.Speed));
        //weaponData.CurrentAmmo--;
    }

    private IEnumerator DespawnBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);

        if (currentBulletsPool != null)
        {
            currentBulletsPool.Despawn(bullet);
        }
        else
        {
            Debug.LogError("PlayerShoot: Object Pool �� ������ ��� ������� ������� ����!");
        }
    }
}
