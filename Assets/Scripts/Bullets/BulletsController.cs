using System.Collections;

using UnityEngine;

public class BulletsController : MonoBehaviour
{
    [SerializeField] public BulletsData bulletData;

    private ObjectPool objectPool;
    private Rigidbody rb;
    private Coroutine despawnCoroutine;
    private WeaponData weaponData;

    private int enemyLayer;
    private int environmentLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"BulletsController: Rigidbody не найден на {gameObject.name}");
        }

        enemyLayer = LayerMask.NameToLayer("Enemy");
        environmentLayer = LayerMask.NameToLayer("Environment");
    }

    private void OnEnable()
    {
        //Debug.Log($"BulletsController: {gameObject.name} активирована из пула. ObjectPool = {objectPool?.gameObject.name}");
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
            Debug.LogError($"BulletsController: Получен NULL пул для {gameObject.name}!");
            return;
        }
        objectPool = pool;
        weaponData = weapon;
        //Debug.Log($"BulletsController: {gameObject.name} привязан к пулу {pool.gameObject.name}");
        rb.velocity = direction.normalized * bulletData.Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer || other.gameObject.layer == environmentLayer)
        {
            HealthController enemyHealth = other.GetComponentInParent<HealthController>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(weaponData.Damage);

                DespawnBullet();
                return;
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
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
        if (objectPool != null)
        {
            objectPool.Despawn(gameObject);
        }
        else
        {
            Debug.LogError($"BulletsController: ObjectPool не назначен для {gameObject.name}!");
        }
    }
}

