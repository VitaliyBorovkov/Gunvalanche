using UnityEngine;

public class EnemyHealthController : HealthController
{
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
            Debug.Log($"EnemyHealthController: Spawned damage text at {damageTextSpawnPoint.position}");
        }
    }

    protected override void Die()
    {
        base.Die();

        if (enemyPool != null)
        {
            enemyPool.Despawn(gameObject);
            //Debug.Log($"EnemyHealthController: {gameObject.name} был возвращён в пул.");
        }
        else
        {
            Debug.LogWarning($"EnemyHealthController: ObjectPool не найден для {gameObject.name}!");
        }
    }
}
