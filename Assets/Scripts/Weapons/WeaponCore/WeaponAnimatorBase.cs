using System.Collections;

using UnityEngine;

public abstract class WeaponAnimatorBase : MonoBehaviour
{
    protected IEnumerator AnimateRotation(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        transform.localRotation = to;
    }

    protected IEnumerator AnimatePosition(Transform target, Vector3 from, Vector3 to, float duration)
    {
        if (target == null)
        {
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.localPosition = Vector3.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        target.localPosition = to;
    }
}
