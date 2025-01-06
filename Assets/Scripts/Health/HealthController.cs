using System;

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
                Debug.Log("PlayerHealthController: PlayerHpUI íå íàéäåí â ñöåíå!");
            }
            else
            {
                playerHpUI.Initialize(entityData.Health);
            }
        }
    }

    private void OnEnable()
    {
        healthData.CurrentHealth = entityData.Health;
        Debug.Log($"{entityData.Name} spawned with {healthData.CurrentHealth} health.");

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
            Debug.Log($"{entityData.Name} óæå ì¸ðòâ, óðîí íå ïðèìåíÿåòñÿ.");
            return;
        }

        healthData.CurrentHealth -= damage;
        Debug.Log($"{entityData.Name} took {damage} damage. Health: {healthData.CurrentHealth}");

        UpdateHeadUI();

        if (healthData.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void SetHealth(int health)
    {
        healthData.CurrentHealth = Mathf.Clamp(health, 0, entityData.Health);
        Debug.Log($"{entityData.Name} health set to: {healthData.CurrentHealth}");

        UpdateHeadUI();
    }

    protected virtual void Die()
    {
        Debug.Log($"{entityData.Name} has died.");
        healthData.OnEndedHealth?.Invoke();


        gameObject.SetActive(false);

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