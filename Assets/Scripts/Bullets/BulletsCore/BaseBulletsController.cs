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

        // The bullet always travels from the muzzle towards the aimed-at point (needed so
        // it actually reaches whatever is under the crosshair, including targets standing
        // right next to the player). It spawns facing the muzzle's own orientation though,
        // so at steep angles (close targets) the model visibly flew off in a direction it
        // wasn't facing — fixed by orienting the model to match its real travel direction.
        Vector3 velocityDirection = direction.normalized;
        transform.rotation = Quaternion.LookRotation(velocityDirection);

        if (rigidBody != null)
        {
            rigidBody.velocity = velocityDirection * bulletsData.Speed;
        }

        if (bulletsData.LifeTime > 0)
        {
            despawnCoroutine = StartCoroutine(DespawnAfterTime(bulletsData.LifeTime));
        }
    }

    // Used for point-blank shots: the muzzle sits offset from the camera, so a target
    // standing right next to the player can be closer than that offset — no direction fired
    // from the muzzle can geometrically reach such a target (see DECISIONS.md). Rather than
    // let the projectile fly on an approximated direction and possibly hit something else
    // entirely, it's resolved immediately at the aim raycast's own (unoffset, always exact)
    // hit point, going through the exact same hit-resolution path a normal collision would.
    public virtual void ResolveImmediately(ObjectPool pool, WeaponData weapon, Vector3 position, Collider hitCollider)
    {
        objectPool = pool;
        weaponData = weapon;

        transform.position = position;

        if (rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
        }

        HandleHit(hitCollider);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    protected virtual void HandleHit(Collider other)
    {
        // Bullets never interact with each other — without this, a weapon that spawns
        // several projectiles from the same point in the same frame (the shotgun's 8
        // pellets) sees them all overlapping one another the instant they appear, and
        // DespawnBullet() below runs unconditionally on any trigger hit regardless of
        // layer, so they'd wipe each other out before ever leaving the muzzle.
        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }

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

