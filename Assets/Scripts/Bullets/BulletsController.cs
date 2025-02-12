using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsController : MonoBehaviour
{
    [SerializeField] public BulletsData bulletData;

    private ObjectPool objectPool;
    private Rigidbody rb;
    private Coroutine despawnCoroutine;
    private WeaponData weaponData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"BulletsController: Rigidbody �� ������ �� {gameObject.name}");
        }
    }
    //private void Update()
    //{
    //    MoveBullet(bulletData);
    //}

    private void OnEnable()
    {
        Debug.Log($"BulletsController: {gameObject.name} ������������ �� ����. ObjectPool = {objectPool?.gameObject.name}");
        if (bulletData.LifeTime > 0)
        {
            despawnCoroutine = StartCoroutine(DespawnAfterTime(bulletData.LifeTime));
        }
    }

    private void OnDisable()
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
        }
    }

    public void Initialize(Vector3 direction, ObjectPool pool, WeaponData weapon)
    {
        if (pool == null)
        {
            Debug.LogError($"BulletsController: ������� NULL ��� ��� {gameObject.name}!");
            return;
        }
        objectPool = pool;
        weaponData = weapon;
        Debug.Log($"BulletsController: {gameObject.name} �������� � ���� {pool.gameObject.name}");
        rb.velocity = direction.normalized * bulletData.Speed;
    }

    //public void SetPool(ObjectPool pool)
    //{
    //    objectPool = pool;
    //    Debug.Log($"BulletsController: {gameObject.name} �������� � ���� {pool.gameObject.name}");
    //}

    //private void MoveBullet(BulletsData bulletData)
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, bulletData.Target, bulletData.Speed * Time.deltaTime);
    //    if (!bulletData.HitTarget && Vector3.Distance(transform.position, bulletData.Target) < bulletData.DistanceToTarget)
    //    {
    //        bulletData.HitTarget = true;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Environment"))
        {
            //if (objectPool != null)
            //{
            //    Debug.Log($"BulletsController: {gameObject.name} ���������� � ��� {objectPool.gameObject.name}");
            //    DespawnBullet();
            //}
            //else
            //{
            //    Debug.LogError($"BulletsController: ObjectPool �� �������� ��� {gameObject.name}!");
            //}
            HealthController enemyHealth = other.GetComponentInParent<HealthController>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(weaponData.Damage);
            }
        }
        DespawnBullet();
    }

    private IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DespawnBullet();
    }

    private void DespawnBullet()
    {
        //if (objectPool == null)
        //{
        //    Debug.LogError($"BulletsController: ObjectPool �� ������ ��� ������� �������� {gameObject.name}!");
        //    return;
        //}
        //gameObject.SetActive(false);
        //objectPool.Despawn(gameObject);
        //Debug.Log($"BulletsController: {gameObject.name} ������� ���������� � {objectPool.gameObject.name}");

        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
        if (objectPool != null)
        {
            objectPool.Despawn(gameObject);
        }
        else
        {
            Debug.LogError($"BulletsController: ObjectPool �� �������� ��� {gameObject.name}!");
        }
    }
}

