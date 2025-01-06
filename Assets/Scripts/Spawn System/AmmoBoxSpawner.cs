using UnityEngine;

public class AmmoBoxSpawner : ObjectSpawner
{
    [Header("Ammo Box Settings")]
    [SerializeField] private GameObject ammoBoxPrefab;

    protected override void SpawnObject()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(ammoBoxPrefab, spawnPoint.position, Quaternion.identity);
    }

    //protected override int CountActiveObjects()
    //{
    //    return GameObject.FindObjectsOfType<AmmoBox>().Length;
    //}
}

