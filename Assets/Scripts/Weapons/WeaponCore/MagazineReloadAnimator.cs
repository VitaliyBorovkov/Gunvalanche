using System.Collections;

using UnityEngine;

public class MagazineReloadAnimator : WeaponAnimatorBase
{
    [SerializeField] private Transform magazineTransform;
    [SerializeField] private Vector3 insertStartLocalOffset = new Vector3(0f, -0.3f, 0f);
    [SerializeField] private float ejectDelay = 0.05f;
    [SerializeField] private float ejectPhaseFraction = 0.15f;
    [SerializeField] private float insertPhaseFraction = 0.2f;
    [SerializeField] private float untiltPhaseFraction = 0.15f;

    [SerializeField] private Vector3 tiltLocalEulerAngles = new Vector3(0f, 0f, 25f);

    [SerializeField] private GameObject magazinePoolPrefab;
    [SerializeField] private float dropSpeed = 1f;
    [SerializeField] private float dropSpeedVariance = 0.2f;
    [SerializeField] private float maxAngularSpeed = 360f;

    private Vector3 restLocalPosition;
    private Quaternion restLocalRotation;
    private Renderer magazineRenderer;
    private ObjectPool magazinePool;
    private PlayerReload playerReload;
    private WeaponSway weaponSway;
    private Coroutine reloadCoroutine;

    private void Awake()
    {
        if (magazineTransform != null)
        {
            restLocalPosition = magazineTransform.localPosition;
            magazineRenderer = magazineTransform.GetComponentInChildren<Renderer>();
        }
        restLocalRotation = transform.localRotation;
        weaponSway = GetComponentInParent<WeaponSway>();
        playerReload = GetComponentInParent<PlayerReload>();

        if (magazinePoolPrefab != null)
        {
            GameObject poolObject = Instantiate(magazinePoolPrefab);
            magazinePool = poolObject.GetComponent<ObjectPool>();
        }
    }

    private void OnEnable()
    {
        if (playerReload != null)
        {
            playerReload.OnReloadStarted += HandleReloadStarted;
        }
    }

    private void OnDisable()
    {
        if (playerReload != null)
        {
            playerReload.OnReloadStarted -= HandleReloadStarted;
        }

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        // A weapon swap mid-reload shouldn't leave the gun tilted or the magazine missing next
        // time it's drawn.
        if (magazineTransform != null)
        {
            magazineTransform.localPosition = restLocalPosition;
        }
        if (magazineRenderer != null)
        {
            magazineRenderer.enabled = true;
        }
        transform.localRotation = restLocalRotation;

        if (weaponSway != null)
        {
            weaponSway.enabled = true;
        }
    }

    private void HandleReloadStarted()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }
        reloadCoroutine = StartCoroutine(ReloadRoutine(playerReload.ReloadDuration));
    }

    private IEnumerator ReloadRoutine(float duration)
    {
        if (weaponSway != null)
        {
            weaponSway.enabled = false;
        }

        float ejectTime = Mathf.Max(0.01f, duration * ejectPhaseFraction);
        float insertTime = Mathf.Max(0.01f, duration * insertPhaseFraction);
        float untiltTime = Mathf.Max(0.01f, duration * untiltPhaseFraction);
        float emptyTime = Mathf.Max(0f, duration - ejectTime - insertTime - untiltTime);

        // The rifle starts tilting immediately, but the magazine itself detaches a beat later
        // (as if the mag release is pressed partway through the motion) rather than the instant
        // the reload begins — a tiny stagger reads as more natural than both starting at once.
        Quaternion tiltedRotation = restLocalRotation * Quaternion.Euler(tiltLocalEulerAngles);
        Coroutine tiltInRoutine = StartCoroutine(AnimateRotation(restLocalRotation, tiltedRotation, ejectTime));

        float clampedEjectDelay = Mathf.Clamp(ejectDelay, 0f, ejectTime);
        if (clampedEjectDelay > 0f)
        {
            yield return new WaitForSeconds(clampedEjectDelay);
        }

        EjectMagazine();
        if (magazineRenderer != null)
        {
            magazineRenderer.enabled = false;
        }

        yield return tiltInRoutine;

        if (emptyTime > 0f)
        {
            yield return new WaitForSeconds(emptyTime);
        }

        // The new magazine slides into place while the rifle is still tilted — only once it's
        // fully seated does the rifle straighten back out, matching a real reload's timing
        // instead of both happening at once.
        Vector3 insertStartPosition = restLocalPosition + insertStartLocalOffset;
        if (magazineTransform != null)
        {
            magazineTransform.localPosition = insertStartPosition;
        }
        if (magazineRenderer != null)
        {
            magazineRenderer.enabled = true;
        }

        yield return AnimatePosition(magazineTransform, insertStartPosition, restLocalPosition, insertTime);
        yield return AnimateRotation(tiltedRotation, restLocalRotation, untiltTime);

        if (weaponSway != null)
        {
            weaponSway.enabled = true;
        }

        reloadCoroutine = null;
    }

    private void EjectMagazine()
    {
        if (magazinePool == null || magazineTransform == null)
        {
            return;
        }

        GameObject dropped = magazinePool.Spawn(magazineTransform.position, magazineTransform.rotation, true);

        if (!dropped.TryGetComponent(out EjectedCasing ejectedMagazine))
        {
            return;
        }

        float speed = dropSpeed + Random.Range(-dropSpeedVariance, dropSpeedVariance);
        Vector3 velocity = -magazineTransform.up * speed;
        Vector3 angularVelocity = Random.insideUnitSphere * maxAngularSpeed * Mathf.Deg2Rad;

        ejectedMagazine.Launch(magazinePool, velocity, angularVelocity);
    }
}
