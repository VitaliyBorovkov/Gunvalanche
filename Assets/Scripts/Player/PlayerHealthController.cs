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
