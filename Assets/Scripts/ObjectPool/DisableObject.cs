using System.Collections;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    [SerializeField] private float time;

    private void OnEnable()
    {
        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
