using System.Collections;

using UnityEngine;

public class EjectedCasing : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody rb;
    private ObjectPool objectPool;
    private Coroutine despawnCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(ObjectPool pool, Vector3 velocity, Vector3 angularVelocity)
    {
        objectPool = pool;

        if (rb != null)
        {
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }

        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
        }
        despawnCoroutine = StartCoroutine(DespawnAfterTime(lifeTime));
    }

    private IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Despawn();
    }

    private void Despawn()
    {
        despawnCoroutine = null;

        gameObject.SetActive(false);

        if (objectPool != null)
        {
            objectPool.Despawn(gameObject);
            objectPool = null;
        }
    }

    private void OnDisable()
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }
    }
}
