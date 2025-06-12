using System.Collections;

using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private IWeapon currentWeapon;

    private Coroutine shootingCoroutine;

    public void StartFiring()
    {
        if (currentWeapon == null || !currentWeapon.CanShoot())
        {
            Debug.Log("PlayerShoot: No weapon or cannot shoot.");
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

    public IWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }


    //    [SerializeField] private Transform WeaponsHolder;
    //    [SerializeField] private PlayerReload playerReload;
    //    [SerializeField] private BulletsConfig bulletsConfig;

    //private WeaponData weaponData;
    //    private BulletsData bulletsData;
    //    private GameObject currentWeapon;
    //    private Transform cameraTransform;
    //    private ObjectPool currentBulletsPool;
    //private WeaponConfigHolder weaponConfigHolder;

    //    private bool IsFiring = false;

    //    private void Awake()
    //    {
    //        cameraTransform = Camera.main.transform;
    //    }

    //    private void Start()
    //    {
    //        if (WeaponsHolder != null && WeaponsHolder.transform.childCount > 0)
    //        {
    //            SetCurrentWeapon(WeaponsHolder.transform.GetChild(0).gameObject);
    //        }

    //        if (currentWeapon != null)
    //        {
    //            SetCurrentWeapon(currentWeapon);
    //        }
    //    }

    //    public void StartFiring()
    //    {
    //        if (currentWeapon == null)
    //        {

    //            Debug.LogWarning("PlayerShoot: Нет активного оружия!");
    //            return;
    //        }

    //        weaponData = GetWeaponData();
    //        if (weaponData.CurrentAmmo > 0 && !playerReload.IsReloading())
    //        {
    //            IsFiring = true;
    //            StartCoroutine(ShootAuto());
    //        }
    //    }

    //    public void StopFiring()
    //    {
    //        IsFiring = false;
    //        StopAllCoroutines();
    //    }

    //    private IEnumerator ShootAuto()
    //    {
    //        weaponData = GetWeaponData();

    //        while (IsFiring && weaponData.CurrentAmmo > 0)
    //        {
    //            ShootGun();
    //            yield return new WaitForSeconds(1f / weaponData.FireRate);
    //        }

    //        StopFiring();
    //    }

    public void SetCurrentWeapon(IWeapon weapon)
    {
        currentWeapon = weapon;
        Debug.Log($"PlayerShoot: Current weapon set to {((MonoBehaviour)weapon).gameObject.name}");

        //weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();

        //if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        //{
        //    Debug.LogError($"PlayerShoot: {weapon.name} не найден в WeaponConfigHolder!");
        //    return;
        //}

        //weaponData = weaponConfigHolder.weaponConfig.weaponData[0];

        //currentBulletsPool = AmmoManager.Instance.GetBulletsPool(weaponData.BulletsType);
        //if (currentBulletsPool == null)
        //{
        //    Debug.LogError($"PlayerShoot: Нет пула для типа {weaponData.BulletsType}");
        //}

        //bulletsData = GetBulletDataForWeapon(weaponData.BulletsType);

        //UpdateWeaponData();
    }

    //    private BulletsData GetBulletDataForWeapon(BulletsType bulletsType)
    //    {
    //        foreach (var bullet in bulletsConfig.bulletsData)
    //        {
    //            if (bullet.BulletsType == bulletsType)
    //            {
    //                return bullet;
    //            }
    //        }
    //        Debug.LogWarning($"PlayerShoot: Не найден BulletsData для {bulletsType}");
    //        return new BulletsData();
    //    }

    //public GameObject GetCurrentWeapon()
    //{
    //    return currentWeapon;
    //}

    //public WeaponData GetWeaponData()
    //{
    //if (currentWeapon == null)
    //{
    //    return new WeaponData();
    //}

    //weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();
    //if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
    //{
    //    return new WeaponData();
    //}

    //return weaponConfigHolder.weaponConfig.weaponData[0];

    //    return new WeaponData();
    //}

    public void UpdateWeaponData()
    {
        //if (currentWeapon == null)
        //{
        //    return;
        //}

        //weaponConfigHolder = currentWeapon.GetComponent<WeaponConfigHolder>();
        //if (weaponConfigHolder == null || weaponConfigHolder.weaponConfig == null)
        //{
        //    return;
        //}

        //weaponData = weaponConfigHolder.weaponConfig.weaponData[0];
    }

    //    public void ShootGun()
    //    {
    //        if (currentWeapon == null || currentBulletsPool == null)
    //        {
    //            Debug.LogWarning("PlayerShoot: Нет активного оружия или пул не назначен!");
    //            return;
    //        }

    //        weaponData = GetWeaponData();
    //        if (weaponData == null)
    //        {
    //            Debug.LogWarning("PlayerShoot: Попытка выстрелить без патронов!");
    //            return;
    //        }

    //        weaponData.CurrentAmmo--;

    //        Transform spawnPoint = currentWeapon.GetComponent<WeaponConfigHolder>().bulletSpawnPoint;
    //        if (spawnPoint == null)
    //        {
    //            Debug.LogWarning("PlayerShoot: Bullet Spawn Point не назначен!");
    //            return;
    //        }


    //        //Debug.Log($"PlayerShoot: Оружие {weaponData.Name}, используется пул {currentBulletsPool.gameObject.name}");

    //        GameObject bullet = currentBulletsPool.Spawn(spawnPoint.position, spawnPoint.rotation);
    //        bullet.transform.forward = spawnPoint.forward;
    //        if (bullet == null)
    //        {
    //            Debug.LogWarning("PlayerShoot: Object Pool оказался null для текущего оружия!");
    //            return;
    //        }

    //        BulletsController bulletsController = bullet.GetComponent<BulletsController>();
    //        if (bulletsController == null)
    //        {
    //            Debug.LogWarning("PlayerShoot: У компонента пули нет BulletsController!");
    //            return;
    //        }

    //        Vector3 shootDirection;

    //        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //        RaycastHit hitInfo;
    //        if (Physics.Raycast(ray, out hitInfo, weaponData.Range))
    //        {
    //            shootDirection = (hitInfo.point - spawnPoint.position).normalized;
    //        }
    //        else
    //        {
    //            shootDirection = ray.direction;
    //        }


    //        if (currentBulletsPool == null)
    //        {
    //            Debug.LogError($"PlayerShoot: Object Pool не найден для {weaponData.BulletsType}");
    //            return;
    //        }

    //        bulletsController.Initialize(shootDirection, currentBulletsPool, weaponData, bulletsConfig);

    //        if (weaponData.MuzzleFlashPrefab != null && spawnPoint != null)
    //        {
    //            GameObject muzzleFlash = Instantiate(weaponData.MuzzleFlashPrefab, spawnPoint.position, spawnPoint.rotation,
    //                spawnPoint);

    //            ParticleSystem particalSystem = muzzleFlash.GetComponent<ParticleSystem>();
    //            if (particalSystem != null)
    //            {
    //                particalSystem.Play();
    //                Destroy(muzzleFlash, particalSystem.main.duration);
    //            }
    //            else
    //            {
    //                Debug.LogWarning($"PlayerShoot: {weaponData.MuzzleFlashPrefab.name} не имеет ParticalSystem!");
    //                Destroy(muzzleFlash, 1f);
    //            }
    //        }
    //    }
}
