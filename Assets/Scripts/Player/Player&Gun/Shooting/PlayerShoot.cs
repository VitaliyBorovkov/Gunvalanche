using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] public WeaponData[] weaponData;
    [SerializeField] private BulletsData bulletData;
    [SerializeField] private PlayerReload playerReload;

    private ObjectPool bulletsPool;

    private bool IsFiring = false;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
    
    public void StartFiring() 
    {
        if (weaponData[0].CurrentAmmo > 0 && !playerReload.IsReloading())
        {
            IsFiring = true;
            StartCoroutine(ShootAuto());
        }
    }

    public void StopFiring()
    {
        IsFiring = false;
        StopCoroutine(ShootAuto());
    }

    private IEnumerator ShootAuto()
    {
        while (IsFiring && weaponData[0].CurrentAmmo > 0)
        {
            ShootGun();
            yield return new WaitForSeconds(1f / weaponData[0].FireRate);
        }

        if (weaponData[0].CurrentAmmo <= 0)
        {
            StopFiring();
        }
    }

    public void SetObjectPool(ObjectPool pool)
    {
        bulletsPool = pool;
    }

    public void ShootGun()
    {
        if (weaponData.Length > 0)
        {
            GameObject bullet = bulletsPool.Spawn(weaponData[0].BulletSpawnPoint.position, Quaternion.identity);

            BulletsController bulletsController = bullet.GetComponent<BulletsController>();

            float range = weaponData[0].Range;
            bool hitTarget = false;
            Vector3 target = cameraTransform.position + cameraTransform.forward * range;

            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
            {
                target = hit.point;
                hitTarget = true;

                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Enemy is attacking");
                    HealthController healthController = hit.collider.GetComponentInParent<HealthController>();
                    if (healthController != null)
                    {
                        healthController.TakeDamage(weaponData[0].Damage);
                        Debug.Log($"Damage applied: {weaponData[0].Damage}");
                    }
                }
            }
            bulletsController.bulletData.Target = target;
            bulletsController.bulletData.HitTarget = hitTarget;

            bullet.transform.forward = target - bullet.transform.position;

            StartCoroutine(DespawnBulletAfterTime(bullet, bulletData.LifeTime));
            weaponData[0].CurrentAmmo--;
        }
    }

    private IEnumerator DespawnBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        bulletsPool.Despawn(bullet);
    }
}
