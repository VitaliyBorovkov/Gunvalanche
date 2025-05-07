using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    [SerializeField] protected HealthData healthData;
    [SerializeField] protected EntityData entityData;

    protected PlayerHpUI playerHpUI;
    private bool isPlayer;
    private bool isDead = false;

    protected virtual void Start()
    {
        isPlayer = CompareTag("Player");

        if (isPlayer)
        {
            playerHpUI = FindObjectOfType<PlayerHpUI>();

            if (playerHpUI == null)
            {
                Debug.Log("PlayerHealthController: PlayerHpUI не найден в сцене!");
            }
            else
            {
                playerHpUI.Initialize(entityData.Health);
            }
        }
    }

    private void OnEnable()
    {
        isDead = false;

        healthData.CurrentHealth = entityData.Health;
        //Debug.Log($"HealthController: {entityData.Name} spawned with {healthData.CurrentHealth} health.");

        UpdateHeadUI();
    }

    public int GetCurrentHealth()
    {
        return healthData.CurrentHealth;
    }

    public int GetMaxHealth()
    {
        return healthData.MaxHealth;
    }

    public virtual void TakeDamage(int damage)
    {

        if (healthData.CurrentHealth <= 0)
        {
            return;
        }

        healthData.CurrentHealth -= damage;
        Debug.Log($"HealthController: {entityData.Name} took {damage} damage. Health: {healthData.CurrentHealth}");

        UpdateHeadUI();

        OnDamageTaken(damage);

        if (healthData.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void SetHealth(int health)
    {
        healthData.CurrentHealth = Mathf.Clamp(health, 0, entityData.Health);
        Debug.Log($"HealthController: {entityData.Name} health set to: {healthData.CurrentHealth}");

        UpdateHeadUI();
    }

    protected virtual void Die()
    {
        Debug.Log($"HealthController:{entityData.Name} has died.");
        healthData.OnEndedHealth?.Invoke();

        if (isDead) return;
        isDead = true;
    }

    private void UpdateHeadUI()
    {
        if (isPlayer && playerHpUI != null)
        {
            playerHpUI.UpdateUI(healthData.CurrentHealth);
        }
    }

    protected virtual void OnDamageTaken(int damage)
    {

    }
}