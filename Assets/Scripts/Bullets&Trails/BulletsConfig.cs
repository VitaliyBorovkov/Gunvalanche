using UnityEngine;

[CreateAssetMenu(fileName = "Bullets", menuName = "Bullets/Config", order = 0)]
public class BulletsConfig : ScriptableObject
{
    public BulletsData[] bulletsData;

    //public BulletsData GetBulletsConfig(string key)
    //{
    //    return bulletsData.FirstOrDefault(bullet => bullet.Name == key);
    //}
}
