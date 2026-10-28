using System.Collections;

using UnityEngine;

public class PumpActionAnimator : WeaponAnimatorBase
{
    [SerializeField] private Transform slideTransform;
    [SerializeField] private Vector3 slideLocalOffset = new Vector3(0f, 0f, -0.03f);
    [SerializeField] private Vector3 raisedLocalEulerAngles = new Vector3(-15f, 0f, 0f);

    [SerializeField] private float raiseDuration = 0.08f;
    [SerializeField] private float slideOutDuration = 0.05f;
    [SerializeField] private float slideInDuration = 0.05f;
    [SerializeField] private float lowerDuration = 0.12f;

    private Vector3 restSlideLocalPosition;
    private Quaternion restLocalRotation;
    private Coroutine pumpCoroutine;

    // A shotgun physically can't fire again until it's been racked — WeaponController.CanShoot()
    // checks this to block the next shot outright, not just skip its visual (unlike the pistol's
    // SlideRecoilAnimator, where rapid-clicking past the animation is an intentional feature).
    public bool IsPlaying => pumpCoroutine != null;

    private void Awake()
    {
        if (slideTransform != null)
        {
            restSlideLocalPosition = slideTransform.localPosition;
        }
        restLocalRotation = transform.localRotation;
    }

    public void PlayPumpAction()
    {
        if (pumpCoroutine != null)
        {
            // WeaponController.CanShoot() already blocks firing while IsPlaying is true, so this
            // shouldn't normally trigger — kept as a defensive guard against restarting mid-way
            // (which would snap the barrel/slide back to rest and jump again) if that ever changes.
            return;
        }

        pumpCoroutine = StartCoroutine(PumpRoutine());
    }

    // Barrel rises first (as if worked by hand), the chambering slide moves out-and-back while
    // raised, and only then does the weapon come back down — same "hold the pose while the
    // action happens" sequencing as the rifle's magazine reload, just triggered per shot.
    private IEnumerator PumpRoutine()
    {
        Quaternion raisedRotation = restLocalRotation * Quaternion.Euler(raisedLocalEulerAngles);
        Vector3 slideOutPosition = restSlideLocalPosition + slideLocalOffset;

        yield return AnimateRotation(restLocalRotation, raisedRotation, raiseDuration);
        yield return AnimatePosition(slideTransform, restSlideLocalPosition, slideOutPosition, slideOutDuration);
        yield return AnimatePosition(slideTransform, slideOutPosition, restSlideLocalPosition, slideInDuration);
        yield return AnimateRotation(raisedRotation, restLocalRotation, lowerDuration);

        pumpCoroutine = null;
    }

}
