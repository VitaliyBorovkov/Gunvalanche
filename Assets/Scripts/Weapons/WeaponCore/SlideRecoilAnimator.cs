using System.Collections;

using UnityEngine;

public class SlideRecoilAnimator : WeaponAnimatorBase
{
    [SerializeField] private Transform slideTransform;
    [SerializeField] private Vector3 recoilLocalOffset = new Vector3(0f, 0f, -0.05f);
    [SerializeField] private float recoilDuration = 0.04f;
    [SerializeField] private float returnDuration = 0.08f;

    private Vector3 restLocalPosition;
    private Coroutine recoilCoroutine;

    private void Awake()
    {
        if (slideTransform != null)
        {
            restLocalPosition = slideTransform.localPosition;
        }
    }

    public void PlayRecoil()
    {
        if (slideTransform == null || recoilCoroutine != null)
        {
            // Firing faster than the recoil animation's own length (rapid clicking on a
            // semi-auto weapon is intentional, not a bug — see DECISIONS.md) shouldn't restart
            // it mid-way, which reads as the slide snapping back to rest and jumping again.
            // Simplest stable behavior: let the current play-through finish and skip the visual
            // for the shots that land inside it; the actual shot/damage timing is untouched.
            return;
        }

        recoilCoroutine = StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        Vector3 recoiledPosition = restLocalPosition + recoilLocalOffset;

        yield return AnimatePosition(slideTransform, slideTransform.localPosition, recoiledPosition, recoilDuration);
        yield return AnimatePosition(slideTransform, recoiledPosition, restLocalPosition, returnDuration);

        recoilCoroutine = null;
    }

}
