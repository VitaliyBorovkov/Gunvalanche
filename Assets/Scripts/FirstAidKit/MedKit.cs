using UnityEngine;

public class MedKit : CollectibleItems
{
    [SerializeField] private int healAmount = 50;

    private ObjectPool medKitPool;

    public void SetMedKitPool(ObjectPool pool)
    {
        medKitPool = pool;
    }

    protected override void Collect(GameObject player)
    {
        PlayerHealthController playerHealthController = player.GetComponent<PlayerHealthController>();

        if (playerHealthController != null)
        {
            if (playerHealthController.GetCurrentHealth() < playerHealthController.GetMaxHealth())
            {
                playerHealthController.Heal(healAmount);
                Debug.Log($"MedKit: {gameObject.name}: ����� {player.name} ������ �� {healAmount} HP.");
                base.Collect(player);

                if (medKitPool != null)
                {
                    medKitPool.Despawn(gameObject);
                }
                else
                {
                    Debug.LogWarning($"MedKit: ObjectPool �� ���������� ��� {gameObject.name}!");
                }

                if (SpawnPointManager.Instance != null && spawnPoint != null)
                {
                    SpawnPointManager.Instance.ReleasePoint(spawnPoint);
                    SpawnPointManager.Instance.SetCooldown(spawnPoint);
                }
                //SpawnPointManager.Instance?.ReleasePoint(transform);
                //SpawnPointManager.Instance?.SetCooldown(transform);
            }
            else
            {
                Debug.LogWarning($"MedKit: {gameObject.name}: � ������ {player.name} ������ �������� � ������� �� ���� ���������.");
            }
        }
        else
        {
            Debug.LogWarning($"MedKit: {player.name} �� ����� PlayerHealthController!", this);
        }
    }

    public void ResetState()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}
