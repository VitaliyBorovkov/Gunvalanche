using System;
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
        WeaponData weaponData = player.GetComponent<PlayerShoot>().GetWeaponData();
        if (weaponData == null) 
        {
            return;
        }

        int currentTotalAmmo = AmmoManager.Instance.GetTotalAmmo(gunsType);
        int maxAmmo = GetMaxAmmoFromConfig(gunsType);

        if (currentTotalAmmo >= maxAmmo)
        {
            Debug.Log($"AmmoBox: Боезапас для {gunsType} уже полный ({currentTotalAmmo}/{maxAmmo}). Ящик не исчезает.");
            return;
        }

        PlayPickUpSound();

        bool ammoAdded = AddAmmoToPlayer(player);
        if (ammoAdded && ammoBoxPool != null)
        {
            ammoBoxPool.Despawn(gameObject);
        }
        else
        {
            Debug.LogWarning($"AmmoBox: ObjectPool не установлен для {gameObject.name}!");
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

        Debug.LogWarning($"AmmoBox: Не найден maxAmmo для {gunsType} в WeaponConfig!");
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
                        Debug.Log($"AmmoBox: Боезапас для {weapon.GunsType} уже полный ({currentTotalAmmo}/{maxAmmo}). Ящик не исчезает.");
                        return false;
                    }

                    AmmoManager.Instance.AddAmmo(weapon.GunsType, ammoInBox, maxAmmo);
                    Debug.Log($"AmmoBox: Игрок подобрал {ammoInBox} патронов для {weapon.GunsType}. Теперь в запасе: {AmmoManager.Instance.GetTotalAmmo(weapon.GunsType)}");
                    return true;
                }
            }
        }

        Debug.LogWarning($"AmmoBox: Оружие с типом {gunsType} не найдено в WeaponConfig!");
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
