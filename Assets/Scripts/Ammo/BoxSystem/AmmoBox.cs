using System;
using UnityEngine;

public class AmmoBox : CollectibleItems
{
    [SerializeField] protected int ammoInBox = 10;
    [SerializeField] protected GunsType ammoType;
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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected override void Collect(GameObject player)
    {
        base.Collect(player);

        PlayPickUpSound();
        AddAmmoToPlayer(player);

        if (ammoBoxPool != null)
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

        //SpawnPointManager.Instance?.ReleasePoint(transform);
        //SpawnPointManager.Instance?.SetCooldown(transform);
    }

    protected virtual void AddAmmoToPlayer(GameObject player)
    {
        
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
