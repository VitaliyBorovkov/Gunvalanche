using UnityEngine;

public class AmmoBox : CollectibleItems
{
    private const string LOG_PREFIX = "AmmoBox: ";

    [Header("Config (preferred)")]
    [SerializeField] private AmmoBoxConfig ammoBoxConfig;

    private AudioSource audioSource;
    private ObjectPool ammoBoxPool;

    internal void SetAmmoBoxPool(ObjectPool pool)
    {
        ammoBoxPool = pool;
    }

    protected override void Start()
    {
        base.Start();

        if (ammoBoxConfig == null)
        {
            Debug.LogError(LOG_PREFIX + $"Config is not set on '{gameObject.name}'. AmmoBox will not work correctly.");
        }

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    protected override void Collect(GameObject player)
    {
        if (ammoBoxConfig == null)
        {
            Debug.LogError(LOG_PREFIX + $"Config is not set on '{gameObject.name}'. Collect aborted.");
            return;
        }

        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
        if (playerShoot == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: {player.name} does not contain PlayerShoot!");
            return;
        }

        var weapon = playerShoot.GetCurrentWeapon();
        if (weapon == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Player {player.name} has no active weapon selected..");
            return;
        }

        WeaponData weaponData = weapon.GetWeaponData();

        if (weaponData/*.Equals(default(WeaponData))*/ == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: Unable to obtain WeaponData from {weapon}.");
            return;
        }

        PlayPickUpSound();

        bool ammoAdded = AddAmmoToPlayer(player);
        if (ammoAdded && ammoBoxPool != null)
        {
            ammoBoxPool.Despawn(gameObject);
        }
        else if (ammoBoxPool == null)
        {
            Debug.LogWarning($"{LOG_PREFIX}: ObjectPool not set for {gameObject.name}!");
        }

        if (SpawnPointManager.Instance != null && spawnPoint != null)
        {
            SpawnPointManager.Instance.ReleasePoint(spawnPoint);
            SpawnPointManager.Instance.SetCooldown(spawnPoint);
        }

        if (player.TryGetComponent<PlayerShoot>(out var playerShoot1))
        {
            var currentWeapon = playerShoot1.GetCurrentWeapon();
            if (currentWeapon is WeaponController weaponController)
            {
                weaponController.InvokeAmmoChanged();
            }
        }
    }

    protected virtual bool AddAmmoToPlayer(GameObject player)
    {
        WeaponConfig[] weaponConfigs = Resources.LoadAll<WeaponConfig>("ScriptableObjects/Weapons");

        foreach (var config in weaponConfigs)
        {
            foreach (var weapon in config.weaponData)
            {
                if (weapon.BulletsType == ammoBoxConfig.BulletsType)
                {
                    int currentTotalAmmo = AmmoManager.Instance.GetTotalAmmo(weapon.GunsType);
                    int maxAmmo = weapon.TotalAmmo;

                    if (currentTotalAmmo >= maxAmmo)
                    {
                        Debug.Log($"{LOG_PREFIX}: The ammunition for {ammoBoxConfig.GunsType} is already full ({currentTotalAmmo}/" +
                            $"{maxAmmo}). The box does not disappear.");
                        return false;
                    }

                    AmmoManager.Instance.AddAmmo(weapon.GunsType, ammoBoxConfig.AmmoInBox, maxAmmo);
                    Debug.Log($"{LOG_PREFIX}: The player has picked up {ammoBoxConfig.AmmoInBox} rounds of ammunition for " +
                        $"{weapon.GunsType}. Now in stock: {AmmoManager.Instance.GetTotalAmmo(weapon.GunsType)}");
                    return true;
                }
            }
        }

        Debug.LogWarning($"{LOG_PREFIX}: Weapons of type {ammoBoxConfig.GunsType} not found in WeaponConfig!");
        return false;
    }

    private void PlayPickUpSound()
    {
        if (ammoBoxConfig.AmmoPickUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(ammoBoxConfig.AmmoPickUpSound);
        }
    }

    public void ResetState()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}
