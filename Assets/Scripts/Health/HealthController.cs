using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthData healthData;
    [SerializeField] private EntityData entityData;

    private void OnEnable()
    {
        healthData.CurrentHealth = entityData.Health;
        Debug.Log($"{entityData.Name} spawned with {healthData.CurrentHealth} health.");
    }

    public int GetCurrentHealth()
    {
        return healthData.CurrentHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        healthData.CurrentHealth -= damage;
        Debug.Log($"{entityData.Name} took {damage} damage. Health: {healthData.CurrentHealth}");

        if (healthData.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void SetHealth(int health)
    {
        healthData.CurrentHealth = Mathf.Clamp(health, 0, entityData.Health);
        Debug.Log($"{entityData.Name} health set to: {healthData.CurrentHealth}");
    }

    protected virtual void Die()
    {
        Debug.Log($"{entityData.Name} has died.");
        healthData.OnEndedHealth?.Invoke();

        gameObject.SetActive(false);
    }
}