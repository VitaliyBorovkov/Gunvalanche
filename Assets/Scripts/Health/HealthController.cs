using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthData[] healthData;
    [SerializeField] private EntityData[] entityData;

    private void OnEnable()
    {
        if (healthData != null && entityData != null &&
            healthData.Length > 0 && entityData.Length > 0)
        {
            for (int i = 0; i < Mathf.Min(healthData.Length, entityData.Length); i++)
            {
                healthData[i].CurrentHealth = entityData[i].Health;
            }
        }
    }

    public int GetCurrentHealth()
    {
        return healthData[0].CurrentHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        healthData[0].CurrentHealth -= damage;
        Debug.Log("EnemyHealth" + healthData[0].CurrentHealth);

        if (healthData[0].CurrentHealth <= 0)
        {
            //healthData[1].OnEndedHealth.Invoke();
        }
    }
}
