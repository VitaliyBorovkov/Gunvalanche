using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Waves/Level Waves Config", fileName = "LevelWavesConfig")]
public class LevelWavesConfig : ScriptableObject
{
    public List<WaveData> waves = new();
}
