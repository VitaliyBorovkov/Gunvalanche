using UnityEngine;

public class PlayerHealthController : HealthController
{
    protected override void Start()
    {
        base.Start();

        UpdateHeadUI();
    }

    protected override void OnDamageTaken(int damage)
    {
        UpdateHeadUI();
    }

    public void Heal(int amount)
    {
        if (healthData.CurrentHealth <= 0)
        {
            Debug.Log("Heal() отменён: игрок мёртв!");
            return;
        }

        healthData.CurrentHealth = Mathf.Clamp(healthData.CurrentHealth + amount, 0, healthData.MaxHealth);

        UpdateHeadUI();
    }

    private void UpdateHeadUI()
    {
        if (playerHpUI != null)
        {
            playerHpUI.UpdateUI(healthData.CurrentHealth);
        }
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player has died!");
    }
}
