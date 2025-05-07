using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MedkitSpawner : ObjectSpawner
{
    [Header("MedKit Settings")]

    [SerializeField] private GameObject medKitPrefab;
    [SerializeField] private ObjectPool medKitPool;

    protected override void SpawnObject()
    {
        Transform spawnPoint = GetAvailableSpawnPoint(spawnPointManager, checkRadius, typeof(MedKit));
        if (spawnPoint == null)
        {
            return;
        }

        GameObject spawnedMedKit = medKitPool.Spawn(spawnPoint.position, Quaternion.identity);
        MedKit medKit = spawnedMedKit.GetComponent<MedKit>();
        if (medKit != null)
        {
            medKit.SetMedKitPool(medKitPool);
            medKit.SetSpawnPoint(spawnPoint);
            spawnPointManager.OccupyPoint(spawnPoint, "MedKit");
        }
        else
        {
            Debug.LogWarning($"MedkitSpawner: У объекта {spawnedMedKit.name} отсутствует компонент MedKit!");
        }
    }

    protected override int CountActiveObjects()
    {
        return medKitPool != null ? medKitPool.CountActiveObjects() : 0;
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var point in spawnPoints)
            {
                Gizmos.DrawWireSphere(point.position, checkRadius);
            }
        }
    }
}
