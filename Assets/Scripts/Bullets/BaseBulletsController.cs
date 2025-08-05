using System.Collections;

using UnityEngine;

public class BaseBulletsController : MonoBehaviour, IBullet
{
    protected BulletsData bulletsData;

    protected Rigidbody rb;
    protected ObjectPool objectPool;
    protected WeaponData weaponData;
    protected Coroutine despawnCoroutine;

    protected int enemyLayer;
    protected int environmentLayer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"BulletsController: Rigidbody not found on {gameObject.name}");
        }

        enemyLayer = LayerMask.NameToLayer("Enemy");
        environmentLayer = LayerMask.NameToLayer("Environment");
    }

    //private void OnEnable()
    //{
    //    //Debug.Log($"BulletsController: {gameObject.name} активирована из пула. ObjectPool = {objectPool?.gameObject.name}");
    //    if (bulletsData.LifeTime > 0)
    //    {
    //        despawnCoroutine = StartCoroutine(DespawnAfterTime(bulletsData.LifeTime));
    //    }
    //}

    public virtual void Initialize(Vector3 direction, ObjectPool pool, WeaponData weapon, BulletsData bullets)
    {
        if (pool == null)
        {
            Debug.LogError($"BulletsController: Received NULL pool for {gameObject.name}!");
            return;
        }
        objectPool = pool;
        weaponData = weapon;
        bulletsData = bullets;

        if (bulletsData.BulletPrefab == null)
        {
            Debug.LogError($"BulletsController: Not found BulletsData for {weapon.BulletsType} in {gameObject.name}!");
            return;
        }

        rb.velocity = direction.normalized * bulletsData.Speed;

        if (bulletsData.LifeTime > 0)
        {
            despawnCoroutine = StartCoroutine(DespawnAfterTime(bulletsData.LifeTime));
        }
    }

    protected virtual void OnDisable()
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }

        weaponData = null;
        //objectPool = null;
    }

    protected virtual void OnTriggerEnter(Collider other)
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

    protected virtual IEnumerator DespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DespawnBullet();
    }

    public virtual void DespawnBullet()
    {
        rb.velocity = Vector3.zero;

        DespawnEffect();

        gameObject.SetActive(false);

        if (objectPool != null)
        {
            objectPool.Despawn(gameObject);
            objectPool = null;
        }
        else
        {
            Debug.LogError($"BulletsController: ObjectPool is not assigned to {gameObject.name}!");
        }
    }

    protected virtual void DespawnEffect()
    {
    }
}

