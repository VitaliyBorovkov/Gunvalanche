using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private BulletsData bulletData;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private PlayerReload playerReload;
    [SerializeField] private PlayerInventory playerInventory;

    private bool isFiring = false;
    private Transform cameraTransform;
    private WeaponData? currentWeapon;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        currentWeapon = playerInventory.GetCurrentWeapon();
    }
    
    public void StartFiring() 
    {
        Debug.Log(" PlayerShoot. Current weapon: " + (currentWeapon?.Name ?? "None"));

        if (currentWeapon != null && currentWeapon.Value.CurrentAmmo > 0 && !playerReload.IsReloading())
        {
            isFiring = true;
            StartCoroutine(ShootAuto());
        }
        else
        {
            Debug.Log(" PlayerShoot. Cannot start firing: weapon is null, out of ammo, or reloading.");
        }
    }

    public void StopFiring()
    {
        isFiring = false;
        if (IsInvoking(nameof(ShootAuto)))
        {
            StopCoroutine(nameof(ShootAuto));
        }
    }

    private IEnumerator ShootAuto()
    {
        while (isFiring && currentWeapon != null && currentWeapon.Value.CurrentAmmo > 0)
        {
            ShootGun();
            yield return new WaitForSeconds(1f / currentWeapon.Value.FireRate);
        }

        if (currentWeapon != null && currentWeapon.Value.CurrentAmmo <= 0)
        {
            Debug.Log(" PlayerShoot. Out of ammo, stopping firing.");
            StopFiring();
        }
    }

    public void ShootGun()
    {
        if (currentWeapon.HasValue && currentWeapon.Value.CurrentAmmo > 0)
        {
            GameObject bullet = objectPool.Spawn(currentWeapon.Value.BulletSpawnPoint.position, Quaternion.identity);

            if (bullet == null)
            {
                Debug.Log(" PlayerShoot. ObjectPool returned a null bullet.");
                return;
            }

            BulletsController bulletsController = bullet.GetComponent<BulletsController>();

            if (bulletsController == null)
            {
                Debug.Log(" PlayerShoot. BulletsController component missing on bullet prefab.");
                return;
            }

            float range = currentWeapon.Value.Range;
            bool hitTarget = false;
            Vector3 target = cameraTransform.position + cameraTransform.forward * range;

            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
            {
                target = hit.point;
                hitTarget = true;

                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log(" PlayerShoot. Enemy is attacking");
                    HealthController healthController = hit.collider.GetComponent<HealthController>();
                    if (healthController != null)
                    {
                        healthController.TakeDamage(currentWeapon.Value.Damage);
                    }
                }
            }
            bulletsController.bulletData.Target = target;
            bulletsController.bulletData.HitTarget = hitTarget;

            bullet.transform.forward = target - bullet.transform.position;

            StartCoroutine(DespawnBulletAfterTime(bullet, bulletData.LifeTime));
            playerInventory.UseAmmo(currentWeapon.Value.GunsType, 1);
        }
    }
    private IEnumerator DespawnBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        objectPool.Despawn(bullet);
    }
}
