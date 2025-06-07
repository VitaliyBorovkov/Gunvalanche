using System.Collections;
using System.Linq;

using UnityEngine;

public class BulletsController : MonoBehaviour
{
    private BulletsData bulletsData;

    private Rigidbody rb;
    private ObjectPool objectPool;
    private WeaponData weaponData;
    private Coroutine despawnCoroutine;

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
        if (bulletsData.LifeTime > 0)
        {
            despawnCoroutine = StartCoroutine(DespawnAfterTime(bulletsData.LifeTime));
        }
    }

    private void OnDisable()
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
        }
    }

    public void Initialize(Vector3 direction, ObjectPool pool, WeaponData weapon, BulletsConfig bulletsConfig)
    {
        if (pool == null)
        {
            Debug.LogError($"BulletsController: Получен NULL пул для {gameObject.name}!");
            return;
        }
        objectPool = pool;
        weaponData = weapon;

        bulletsData = bulletsConfig.bulletsData.FirstOrDefault(b => b.BulletsType == weapon.BulletsType);
        if (bulletsData.BulletPrefab == null)
        {
            Debug.LogError($"BulletsController: Не найден BulletsData для {weapon.BulletsType} в {gameObject.name}!");
            return;
        }

        rb.velocity = direction.normalized * bulletsData.Speed;
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

        if (bulletsData.BulletsType == BulletsType.Rocket && bulletsData.ExplosionEffectPrefab != null)
        {
            Instantiate(bulletsData.ExplosionEffectPrefab, transform.position, Quaternion.identity);
        }

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

