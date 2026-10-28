using UnityEngine;

public class CasingEjector : MonoBehaviour
{
    private const string LOG_PREFIX = "CasingEjector";

    [SerializeField] private BulletsType bulletsType;
    [SerializeField] private Transform ejectPoint;
    [SerializeField] private float ejectSpeed = 1.5f;
    [SerializeField] private float ejectSpeedVariance = 0.3f;
    [SerializeField] private float ejectUpwardAngle = 55f;
    [SerializeField] private float ejectSpreadAngle = 12f;
    [SerializeField] private float maxAngularSpeed = 540f;

    private ObjectPool casingPool;

    private void Start()
    {
        if (AmmoManager.Instance != null)
        {
            casingPool = AmmoManager.Instance.GetCasingPool(bulletsType);
        }
    }

    public void Eject()
    {
        if (casingPool == null || ejectPoint == null)
        {
            return;
        }

        GameObject casing = casingPool.Spawn(ejectPoint.position, ejectPoint.rotation, true);

        if (!casing.TryGetComponent(out EjectedCasing ejectedCasing))
        {
            Debug.LogWarning($"{LOG_PREFIX}: {casing.name} has no EjectedCasing component.");
            return;
        }

        // Eject sideways out of the port (ejectPoint.right), tilted up around the port's own
        // forward axis, with a small random cone spread around its up axis so casings don't
        // all fly on the exact same line.
        Vector3 direction = Quaternion.AngleAxis(ejectUpwardAngle, ejectPoint.forward) * ejectPoint.right;
        direction = Quaternion.AngleAxis(Random.Range(-ejectSpreadAngle, ejectSpreadAngle), ejectPoint.up) * direction;

        float speed = ejectSpeed + Random.Range(-ejectSpeedVariance, ejectSpeedVariance);
        Vector3 angularVelocity = Random.insideUnitSphere * maxAngularSpeed * Mathf.Deg2Rad;

        ejectedCasing.Launch(casingPool, direction.normalized * speed, angularVelocity);
    }
}
