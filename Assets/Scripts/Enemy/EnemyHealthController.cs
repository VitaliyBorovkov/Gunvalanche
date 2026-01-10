using UnityEngine;

public class EnemyHealthController : HealthController
{
    private const string LOG_PREFIX = "EnemyHealthController";

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform damageTextSpawnPoint;

    private ObjectPool enemyPool;

    public void SetEnemyPool(ObjectPool pool)
    {
        enemyPool = pool;
    }

    protected override void OnDamageTaken(int damage)
    {
        if (damageTextPrefab != null && damageTextSpawnPoint != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab,
                damageTextSpawnPoint.position, Quaternion.identity);

            //damageText.GetComponent<DamageTextUIController>().SetDamageText(damage);
            Debug.Log($"{LOG_PREFIX}: Spawned damage text at {damageTextSpawnPoint.position}");
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
