using UnityEngine;

[CreateAssetMenu(fileName = "Bullets", menuName = "Bullets/Config", order = 0)]
public class BulletsConfig : ScriptableObject
{
    public BulletsData[] bulletsData;
}
