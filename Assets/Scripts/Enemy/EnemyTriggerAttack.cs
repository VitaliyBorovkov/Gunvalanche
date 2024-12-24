using UnityEngine;

public class EnemyTriggerAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1.5f;

    private float lastAttackTime;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthController playerHealth = other.GetComponent<HealthController>();

            if (playerHealth != null && Time.time >= lastAttackTime + attackCooldown)
            {
                playerHealth.TakeDamage(damage);
                lastAttackTime = Time.time;
                Debug.Log($"EnemyTriggerAttack: Нанесён урон - {damage}. Здоровье игрока: {playerHealth.GetCurrentHealth()}");
            }
        }
    }
}

