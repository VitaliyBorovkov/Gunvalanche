using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private SpawnableObject[] objectsToSpawn;
    [SerializeField] private float spawnInterval = 5f;
    
    private float timer = 0f;

    private void Update()
    {
        timer += Time.time;
        if (timer >= spawnInterval)
        {
            SpawnRandomObject();
            timer = 0f;
        }
    }

    private void SpawnRandomObject()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        SpawnableObject spawnableObject = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];

        Instantiate(spawnableObject.prefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void SpawnSpecificObject(ObjectType objectType, Vector3 position, 
        Quaternion rotation)
    {
        SpawnableObject spawnableObject = System.Array.Find(objectsToSpawn, 
            obj => obj.objectType == objectType);

        if (spawnableObject != null)
        {
            Instantiate(spawnableObject.prefab, position, rotation);
        }
    }
}
