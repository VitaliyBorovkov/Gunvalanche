using System.Collections;

using UnityEngine;

public class ShotgunReloadAnimator : WeaponAnimatorBase
{
    [SerializeField] private Transform shellTransform;
    [SerializeField] private Vector3 shellStartLocalOffset = new Vector3(0f, -0.02f, 0f);

    [SerializeField] private Vector3 tiltLocalEulerAngles = new Vector3(20f, 0f, 0f);
    [SerializeField] private float tiltDuration = 0.15f;
    [SerializeField] private float untiltDuration = 0.15f;

    [SerializeField] private float minShellDuration = 0.15f;
    [SerializeField] private float maxShellDuration = 0.4f;

    private Vector3 shellRestLocalPosition;
    private Quaternion restLocalRotation;
    private PlayerReload playerReload;
    private WeaponSway weaponSway;
    private PumpActionAnimator pumpActionAnimator;
    private Coroutine reloadCoroutine;

    private void Awake()
    {
        if (shellTransform != null)
        {
            shellRestLocalPosition = shellTransform.localPosition;
            shellTransform.gameObject.SetActive(false);
        }
        restLocalRotation = transform.localRotation;
        weaponSway = GetComponentInParent<WeaponSway>();
        pumpActionAnimator = GetComponent<PumpActionAnimator>();
        playerReload = GetComponentInParent<PlayerReload>();
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

        transform.localRotation = restLocalRotation;
        if (shellTransform != null)
        {
            shellTransform.gameObject.SetActive(false);
            shellTransform.localPosition = shellRestLocalPosition;
        }
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
        reloadCoroutine = StartCoroutine(ReloadRoutine(playerReload.PendingReloadAmount, playerReload.ReloadDuration));
    }

    // Tilts the shotgun up once (exposes the loading gate on the underside), feeds shells one at
    // a time while it stays tilted — exactly as many as are actually being loaded, not a fixed
    // count — then straightens back out. Same "hold the pose while the action happens" shape as
    // the rifle's magazine reload and the shotgun's own per-shot pump action.
    private IEnumerator ReloadRoutine(int shellCount, float duration)
    {
        // The last shot before an empty magazine triggers auto-reload synchronously, inside the
        // same WeaponController.Shoot() call that also starts that shot's own pump action — both
        // animations would otherwise fight over this same transform's rotation at once. Wait for
        // the pump to fully finish first: shot -> pump -> tilt -> load -> untilt, never overlapping.
        while (pumpActionAnimator != null && pumpActionAnimator.IsPlaying)
        {
            yield return null;
        }

        if (weaponSway != null)
        {
            weaponSway.enabled = false;
        }

        Quaternion tiltedRotation = restLocalRotation * Quaternion.Euler(tiltLocalEulerAngles);
        yield return AnimateRotation(restLocalRotation, tiltedRotation, tiltDuration);

        if (shellCount > 0)
        {
            float available = Mathf.Max(0.05f, duration - tiltDuration - untiltDuration);
            float perShellDuration = Mathf.Clamp(available / shellCount, minShellDuration, maxShellDuration);

            for (int i = 0; i < shellCount; i++)
            {
                yield return LoadOneShell(perShellDuration);
            }
        }

        yield return AnimateRotation(tiltedRotation, restLocalRotation, untiltDuration);

        if (weaponSway != null)
        {
            weaponSway.enabled = true;
        }

        reloadCoroutine = null;
    }

    private IEnumerator LoadOneShell(float duration)
    {
        if (shellTransform == null)
        {
            yield break;
        }

        Vector3 startPosition = shellRestLocalPosition + shellStartLocalOffset;
        shellTransform.localPosition = startPosition;
        shellTransform.gameObject.SetActive(true);

        float slideDuration = duration * 0.5f;
        yield return AnimatePosition(shellTransform, startPosition, shellRestLocalPosition, slideDuration);

        shellTransform.gameObject.SetActive(false);

        float pauseDuration = duration - slideDuration;
        if (pauseDuration > 0f)
        {
            yield return new WaitForSeconds(pauseDuration);
        }
    }

}
