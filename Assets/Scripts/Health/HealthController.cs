using System;

using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    [SerializeField] protected HealthData healthData;
    [SerializeField] protected EntityData entityData;

    protected PlayerHpUI playerHpUI;
    private bool isPlayer;
    private bool isDead = false;

    public event Action OnDied;
    public bool IsDead => isDead;

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
        if (isDead)
        {
            //Debug.LogWarning($"HealthController: {entityData.Name} is already dead. Cannot take damage.");
            return;
        }

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
        if (isDead)
        {
            return;
        }

        healthData.CurrentHealth = Mathf.Clamp(health, 0, entityData.Health);
        Debug.Log($"HealthController: {entityData.Name} health set to: {healthData.CurrentHealth}");

        UpdateHeadUI();
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log($"HealthController:{entityData.Name} has died.");
        healthData.OnEndedHealth?.Invoke();

        OnDied?.Invoke();
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