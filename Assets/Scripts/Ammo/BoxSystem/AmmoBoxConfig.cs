using UnityEngine;

[CreateAssetMenu(menuName = "Ammo/Ammo Box Config", fileName = "AmmoBoxConfig")]
public class AmmoBoxConfig : ScriptableObject
{
    [Header("Ammo")]
    [SerializeField] private int ammoInBox = 10;
    [SerializeField] private GunsType gunsType;
    [SerializeField] private BulletsType bulletsType;

    [Header("Audio")]
    [SerializeField] private AudioClip ammoPickUpSound;

    public int AmmoInBox => ammoInBox;
    public GunsType GunsType => gunsType;
    public BulletsType BulletsType => bulletsType;
    public AudioClip AmmoPickUpSound => ammoPickUpSound;

}
