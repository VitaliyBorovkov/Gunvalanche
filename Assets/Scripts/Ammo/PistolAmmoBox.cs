using UnityEngine;

public class PistolAmmoBox : AmmoBox
{
    private void Awake()
    {
        ammoType = GunsType.Pistol;
        ammoInBox = 14;
        ammoPickUpSound = Resources.Load<AudioClip>("Sounds/PistolPickup");
    }

    protected override void AddAmmoToPlayer(GameObject player)
    {
        Debug.Log($"PistolAmmoBox: ����� ������� {ammoInBox} �������� ��� ���������.");
    }
}
