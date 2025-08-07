using UnityEngine;

public class AmmoBox : CollectibleItems
{
    [SerializeField] protected int ammoInBox = 10;
    [SerializeField] protected GunsType gunsType;
    [SerializeField] protected BulletsType bulletsType;
    [SerializeField] protected AudioClip ammoPickUpSound;

    private AudioSource audioSource;
    private ObjectPool ammoBoxPool;

    internal void SetAmmoBoxPool(ObjectPool pool)
    {
        ammoBoxPool = pool;
    }

    protected override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    protected override void Collect(GameObject player)
    {

        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
        if (playerShoot == null)
        {
            Debug.LogWarning($"AmmoBox: {player.name} does not contain PlayerShoot!");
            return;
        }

        var weapon = playerShoot.GetCurrentWeapon();
        if (weapon == null)
        {
            Debug.LogWarning($"AmmoBox: Player {player.name} has no active weapon selected..");
            return;
        }

        WeaponData weaponData = weapon.GetWeaponData();
        if (weaponData.Equals(default(WeaponData)))
        {
            Debug.LogWarning($"AmmoBox: Unable to obtain WeaponData from {weapon}.");
            return;
        }

        //int maxAmmo = GetMaxAmmoFromConfig(gunsType);

        PlayPickUpSound();

        bool ammoAdded = AddAmmoToPlayer(player);
        if (ammoAdded && ammoBoxPool != null)
        {
            ammoBoxPool.Despawn(gameObject);
        }
        else
        {
            Debug.LogWarning($"AmmoBox: ObjectPool not set for {gameObject.name}!");
        }

        if (SpawnPointManager.Instance != null && spawnPoint != null)
        {
            SpawnPointManager.Instance.ReleasePoint(spawnPoint);
            SpawnPointManager.Instance.SetCooldown(spawnPoint);
        }
    }

    private int GetMaxAmmoFromConfig(GunsType gunsType)
    {
        WeaponConfig[] weaponConfigs = Resources.LoadAll<WeaponConfig>("ScriptableObjects/Weapons");
        foreach (var config in weaponConfigs)
        {
            foreach (var weapon in config.weaponData)
            {
                if (weapon.GunsType == gunsType)
                {
                    return weapon.TotalAmmo;
                }
            }
        }

        Debug.LogWarning($"AmmoBox: MaxAmmo not found for {gunsType} в WeaponConfig!");
        return 0;
    }

    protected virtual bool AddAmmoToPlayer(GameObject player)
    {
        WeaponConfig[] weaponConfigs = Resources.LoadAll<WeaponConfig>("ScriptableObjects/Weapons");

        foreach (var config in weaponConfigs)
        {
            foreach (var weapon in config.weaponData)
            {
                if (weapon.BulletsType == bulletsType)
                {
                    int currentTotalAmmo = AmmoManager.Instance.GetTotalAmmo(weapon.GunsType);
                    int maxAmmo = weapon.TotalAmmo;

                    if (currentTotalAmmo >= maxAmmo)
                    {
                        Debug.Log($"AmmoBox: The ammunition for {gunsType} is already full ({currentTotalAmmo}/" +
                            $"{maxAmmo}). The box does not disappear.");
                        return false;
                    }

                    AmmoManager.Instance.AddAmmo(weapon.GunsType, ammoInBox, maxAmmo);
                    Debug.Log($"AmmoBox: The player has picked up {ammoInBox} rounds of ammunition for " +
                        $"{weapon.GunsType}. Now in stock: {AmmoManager.Instance.GetTotalAmmo(weapon.GunsType)}");
                    return true;
                }
            }
        }

        Debug.LogWarning($"AmmoBox: Weapons of type {gunsType} not found in WeaponConfig!");
        return false;
    }

    private void PlayPickUpSound()
    {
        if (ammoPickUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(ammoPickUpSound);
        }
    }

    public void ResetState()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}
