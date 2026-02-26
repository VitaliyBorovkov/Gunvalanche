using UnityEngine;

public class ShotgunBulletController : MonoBehaviour
{
    private const string LOG_PREFIX = "ShotgunBulletController";

    [SerializeField] private int pelletsPerShot = 8;
    [SerializeField] private bool uneRadiuseSpread = true;
    [SerializeField] private float spreadAngle = 10f;
    [SerializeField] private float spreadRadiusAtDistance = 0.75f;
    [SerializeField] private float spreadDistance = 10f;

    public void Fire(ObjectPool bulletsPool, Transform spawnPoint, WeaponData weaponData, BulletsData bulletsData,
        Vector3 baseDirection)
    {
        if (bulletsPool == null || spawnPoint == null || weaponData == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Fire() received null dependencies.");
            return;
        }

        if (bulletsData.BulletPrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Fire() bulletsData.BulletPrefab is null. Check BulletsConfig for Shotgun.");
            return;
        }

        if (pelletsPerShot <= 0)
        {
            Debug.LogWarning($"{LOG_PREFIX}: pelletsPerShot <= 0. Forcing to 1.");
            pelletsPerShot = 1;
        }

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 pelletDirection = GetPelletDirection(spawnPoint, baseDirection);

            GameObject pellet = bulletsPool.Spawn(spawnPoint.position, spawnPoint.rotation, true);
            if (pellet.TryGetComponent(out IBullet pelletController))
            {
                pelletController.Initialize(pelletDirection, bulletsPool, weaponData, bulletsData);
            }
            else
            {
                Debug.LogError($"{LOG_PREFIX}: {pellet.name} does not have IBullet component.");
            }
        }
    }

    private Vector3 GetPelletDirection(Transform spawnPoint, Vector3 baseDirection)
    {
        float angleDeg = spreadAngle;

        if (uneRadiuseSpread)
        {
            float angleRad = Mathf.Atan(spreadRadiusAtDistance / spreadDistance);
            angleDeg = angleRad * Mathf.Rad2Deg;
        }

        Vector2 offset = Random.insideUnitCircle * angleDeg;

        Quaternion yaw = Quaternion.AngleAxis(offset.x, spawnPoint.up);
        Quaternion pitch = Quaternion.AngleAxis(offset.y, spawnPoint.right);

        return ((yaw * pitch) * baseDirection).normalized;
    }
}
