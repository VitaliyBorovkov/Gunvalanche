using System;
using UnityEngine;

public class AmmoBox : CollectibleItems
{
    [SerializeField] protected int ammoInBox = 10;
    [SerializeField] protected GunsType ammoType;
    [SerializeField] protected AudioClip ammoPickUpSound;

    private AudioSource audioSource;

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
}
