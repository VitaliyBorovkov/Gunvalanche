using System;

using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    [SerializeField] protected HealthData healthData;
    [SerializeField] protected EntityData entityData;

    private PlayerHpUI playerHpUI;
    private bool isPlayer;

    protected virtual void Start()
    {
        isPlayer = CompareTag("Player");

        if (isPlayer)
        {
            playerHpUI = FindObjectOfType<PlayerHpUI>();

            if (playerHpUI == null)
            {
                Debug.Log("HealthController: HeadHull не найден в сцене!");
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

    public virtual void TakeDamage(int damage)
    {

        if (damage <= 0)
        {
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