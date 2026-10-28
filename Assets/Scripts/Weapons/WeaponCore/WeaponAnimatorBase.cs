using System.Collections;

using UnityEngine;

public abstract class WeaponAnimatorBase : MonoBehaviour
{
    protected IEnumerator AnimateRotation(Transform target, Quaternion from, Quaternion to, float duration)
    {
        if (target == null)
        {
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.localRotation = Quaternion.Slerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        target.localRotation = to;
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
