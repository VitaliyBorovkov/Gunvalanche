using UnityEngine;

public class EnemyHealthController : HealthController
{
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform damageTextSpawnPoint;

    protected override void OnDamageTaken(int damage)
    {
        if (damageTextPrefab != null && damageTextSpawnPoint != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab,
                damageTextSpawnPoint.position, Quaternion.identity);

            //damageText.GetComponent<DamageTextUIController>().SetDamageText(damage);
            Debug.Log($"Spawned damage text at {damageTextSpawnPoint.position}");
        }
    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
}
