using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private WeaponData[] weaponData;
    [SerializeField] private BulletsData[] bulletsData;
    [SerializeField] private ObjectPool objectPool;

    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    public void ShootGun()
    {
        for (int i = 0; i < weaponData.Length; i++)
        {
            if (i < bulletsData.Length)
            {
                //if (weaponData != null && bulletsData != null && weaponData.Length > 0 && bulletsData.Length > 0)
                //{
                //GameObject bullet = Instantiate(bulletsData[0].BulletPrefab, weaponData[0].BulletSpawnPoint.position,
                //    Quaternion.identity, ObjectPool);
                GameObject bullet = objectPool.Spawn(bulletsData[i].BulletPrefab,
                    weaponData[i].BulletSpawnPoint.position, Quaternion.identity);

                BulletsController bulletsController = bullet.GetComponent<BulletsController>();

                float range = weaponData[0].Range;
                bool hitTarget = false;
                Vector3 target = cameraTransform.position + cameraTransform.forward * range;

                RaycastHit hit;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, /*Mathf.Infinity*/range))
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
                //}
            }
        }
    }
}
