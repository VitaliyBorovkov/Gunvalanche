using UnityEngine;

public class MedkitSpawner : ObjectSpawner
{
    [Header("Medkit Settings")]
    [SerializeField] private GameObject medkitPrefab;

    protected override void SpawnObject()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(medkitPrefab, spawnPoint.position, Quaternion.identity);
    }

    //protected override int CountActiveObjects()
    //{
    //    return GameObject.FindObjectsOfType<Medkit>().Length;
    //}
}
