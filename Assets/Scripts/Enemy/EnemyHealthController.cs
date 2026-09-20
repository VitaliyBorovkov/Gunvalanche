using UnityEngine;

public class EnemyHealthController : HealthController
{
    private const string LOG_PREFIX = "EnemyHealthController";

    [SerializeField] private Transform damageTextSpawnPoint;

    private ObjectPool enemyPool;
    private ObjectPool damageTextPool;

    public void SetEnemyPool(ObjectPool pool)
    {
        enemyPool = pool;
    }

    public void SetDamageTextPool(ObjectPool pool)
    {
        damageTextPool = pool;
    }

    protected override void OnDamageTaken(int damage)
    {
        if (damageTextPool != null && damageTextSpawnPoint != null)
        {
            GameObject damageText = damageTextPool.Spawn(damageTextSpawnPoint.position, Quaternion.identity);

            damageText.GetComponent<DamageTextUIController>().Initialize(damageTextPool, damage);
        }
    }

    protected override void Die()
    {
        base.Die();

        if (enemyPool != null)
        {
            enemyPool.Despawn(gameObject);
            //Debug.Log($"{LOG_PREFIX}: {gameObject.name} was returnet to pool.");
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}: ObjectPool not found for {gameObject.name}!");
        }
    }
}
