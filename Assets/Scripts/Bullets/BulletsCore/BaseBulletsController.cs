using System.Collections;

using UnityEngine;

public class BaseBulletsController : MonoBehaviour, IBullet
{
    private const string LOG_PREFIX = "BaseBulletsController";

    protected BulletsData bulletsData;

    protected Rigidbody rigidBody;
    protected ObjectPool objectPool;
    protected WeaponData weaponData;
    protected Coroutine despawnCoroutine;

    protected int enemyLayer;
    protected int environmentLayer;

    private bool isDespawning = false;

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Rigidbody not found on {gameObject.name}");
        }

        enemyLayer = LayerMask.NameToLayer("Enemy");
        environmentLayer = LayerMask.NameToLayer("Environment");
    }

    protected virtual void OnEnable()
    {
        isDespawning = false;
    }

    protected virtual void OnDisable()
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }

        weaponData = null;
    }

    public virtual void Initialize(Vector3 direction, ObjectPool pool, WeaponData weapon, BulletsData bullets)
    {
        if (pool == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Received NULL pool for {gameObject.name}!");
            return;
        }
        objectPool = pool;
        weaponData = weapon;
        bulletsData = bullets;

        if (bulletsData.BulletPrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Not found BulletsData for {weapon.BulletsType} in {gameObject.name}!");
            return;
        }

        if (rigidBody != null)
        {
            rigidBody.velocity = direction.normalized * bulletsData.Speed;
        }

        if (bulletsData.LifeTime > 0)
        {
            despawnCoroutine = StartCoroutine(DespawnAfterTime(bulletsData.LifeTime));
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer || other.gameObject.layer == environmentLayer)
        {
            HealthController enemyHealth = other.GetComponentInParent<HealthController>();
            if (enemyHealth != null && weaponData != null)
            {
                enemyHealth.TakeDamage(weaponData.Damage);
            }
        }
        DespawnBullet();
    }

    protected virtual IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DespawnBullet();
    }

    public virtual void DespawnBullet()
    {
        if (isDespawning)
        {
            Debug.Log($"{LOG_PREFIX}: DespawnBullet called again for {gameObject.name}, ignoring.");
            return;
        }

        isDespawning = true;

        if (rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
        }

        DespawnEffect();

        gameObject.SetActive(false);

        if (objectPool != null)
        {
            objectPool.Despawn(gameObject);
            objectPool = null;
        }
        else
        {
            Debug.LogError($"{LOG_PREFIX}: ObjectPool is not assigned to {gameObject.name}!");
        }
    }

    protected virtual void DespawnEffect()
    {
    }
}

