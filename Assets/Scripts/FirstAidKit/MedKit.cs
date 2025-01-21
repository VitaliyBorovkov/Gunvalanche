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
                Debug.Log($"MedKit: {gameObject.name}: Игрок {player.name} исцелён на {healAmount} HP.");
                base.Collect(player);

                if (medKitPool != null)
                {
                    medKitPool.Despawn(gameObject);
                }
                else
                {
                    Debug.LogWarning($"MedKit: ObjectPool не установлен для {gameObject.name}!");
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
                Debug.LogWarning($"MedKit: {gameObject.name}: У игрока {player.name} полное здоровье и аптечка не была подобрана.");
            }
        }
        else
        {
            Debug.LogWarning($"MedKit: {player.name} не имеет PlayerHealthController!", this);
        }
    }

    public void ResetState()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}
