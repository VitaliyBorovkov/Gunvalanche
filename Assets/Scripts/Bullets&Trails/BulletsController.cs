using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletsController : MonoBehaviour, IPoolable
{
    [SerializeField] public BulletsData[] bulletsData;
    [SerializeField] private ObjectPool objectPool;

    public virtual void OnSpawn()
    {
        Debug.Log("Spawn");
    }

    public virtual void OnDespawn()
    {
        Debug.Log("DeSpawn");
        if (objectPool != null && bulletsData != null && bulletsData.Length > 0)
        {
            foreach (var bulletData in bulletsData)
            {
                StartCoroutine(DespawnWithDelay(bulletData.LifeTime));
            }
        }
    }

    private void Update()
    {
        if (bulletsData != null && bulletsData.Length > 0)
        {
            foreach (var bulletData in bulletsData)
            {
                MoveBullet(bulletData);
            }
        }
    }

    private void MoveBullet(BulletsData bulletData)
    {
        transform.position = Vector3.MoveTowards(transform.position, bulletData.Target, bulletData.Speed * Time.deltaTime);
        if (!bulletData.HitTarget && Vector3.Distance(transform.position, bulletData.Target) < bulletData.DistanceToTarget)
        {
            bulletData.HitTarget = true;
            StartCoroutine(DespawnWithDelay(bulletData.LifeTime));
        }
    }

    private IEnumerator DespawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objectPool != null)
        {
            objectPool.Despawn(gameObject, delay);
        }
    }
}
