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
        Vector3 baseDirection, RaycastHit? pointBlankHit = null)
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
            GameObject pellet = bulletsPool.Spawn(spawnPoint.position, spawnPoint.rotation, true);

            if (!pellet.TryGetComponent(out IBullet pelletController))
            {
                Debug.LogError($"{LOG_PREFIX}: {pellet.name} does not have IBullet component.");
                continue;
            }

            if (pointBlankHit.HasValue)
            {
                // See BaseBulletsController.ResolveImmediately — a target standing this
                // close can be nearer to the camera than the muzzle's own offset from it,
                // so no pellet direction fired from the muzzle can reliably reach it.
                pelletController.ResolveImmediately(bulletsPool, weaponData,
                    pointBlankHit.Value.point, pointBlankHit.Value.collider);
            }
            else
            {
                Vector3 pelletDirection = GetPelletDirection(spawnPoint, baseDirection);
                pelletController.Initialize(pelletDirection, bulletsPool, weaponData, bulletsData);
            }
        }
    }

    private Vector3 GetPelletDirection(Transform spawnPoint, Vector3 baseDirection)
    {
        float maxAngleDeg = spreadAngle;

        if (uneRadiuseSpread)
        {
            float angleRad = Mathf.Atan(spreadRadiusAtDistance / spreadDistance);
            maxAngleDeg = angleRad * Mathf.Rad2Deg;
        }

        // Uniform spread over a circular cone around baseDirection: tilt away from the
        // center by a random angle (sqrt of a uniform value keeps the density even across
        // the disk, not bunched up near the center), then spin that tilt fully around
        // baseDirection itself by a random azimuth. Spinning around baseDirection — not
        // some fixed external axis — is what makes the pattern an actual circle regardless
        // of which way the weapon is pointed; the previous yaw/pitch-around-fixed-axes
        // approach skewed the spread toward the horizontal/vertical axes instead.
        float tiltDeg = maxAngleDeg * Mathf.Sqrt(Random.value);
        float spinDeg = Random.value * 360f;

        Vector3 tiltAxis = Vector3.Cross(baseDirection, spawnPoint.up);
        if (tiltAxis.sqrMagnitude < 0.0001f)
        {
            tiltAxis = spawnPoint.right;
        }

        Quaternion tilt = Quaternion.AngleAxis(tiltDeg, tiltAxis.normalized);
        Quaternion spin = Quaternion.AngleAxis(spinDeg, baseDirection);

        return (spin * tilt * baseDirection).normalized;
    }
}
