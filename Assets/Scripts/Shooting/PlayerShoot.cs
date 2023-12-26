using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private WeaponData[] weaponData;
    [SerializeField] private BulletsData[] bulletsData;
    [SerializeField] private ObjectPool objectPool;

    private bool IsFiring = false;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
    
    public void StartFiring() 
    {
        IsFiring = true;
        StartCoroutine(ShootAuto());
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
    }

    private void ShootBullet() 
    {
        if (!IsFiring)
        {
            ShootGun();
        }
    }

    public void ShootGun()
    {
        for (int i = 0; i < weaponData.Length; i++)
        {
            if (i < bulletsData.Length)
            {
                GameObject bullet = objectPool.Spawn(weaponData[i].BulletSpawnPoint.position, Quaternion.identity);

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
                        HealthController healthController = hit.collider.GetComponent<HealthController>();
                        if (healthController != null)
                        {
                            healthController.TakeDamage(weaponData[1].Damage);
                        }
                    }
                }
                bulletsController.bulletsData[0].Target = target;
                bulletsController.bulletsData[0].HitTarget = hitTarget;

                bullet.transform.forward = target - bullet.transform.position;

                weaponData[0].CurrentAmmo--;
            }
        }
    }
}
