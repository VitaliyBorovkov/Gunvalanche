using System;
using System.Collections.Generic;

[Serializable]
public class WaveSpawn
{
    public int spawnerIndex;
    public int enemyCountInWave = 5;
    public float timeBetweenSpawnEnemyInWave = 1f;
}

[Serializable]
public class WaveData
{
    public string waveName;
    public float startDelay = 0f;
    public List<WaveSpawn> waveSpawns = new();
}
