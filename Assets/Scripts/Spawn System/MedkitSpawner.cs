using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MedkitSpawner : ObjectSpawner
{
    [Header("MedKit Settings")]

    [SerializeField] private GameObject medKitPrefab;
    //[SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private ObjectPool medKitPool;

    //private SpawnPointManager spawnPointManager;

    //private void Start()
    //{
    //    spawnPointManager = FindObjectOfType<SpawnPointManager>();

    //    if (spawnPointManager == null)
    //    {
    //        Debug.Log("MedkitSpawner: SpawnPointManager не найден на сцене!");
    //        enabled = false;
    //        return;
    //    }

    //    spawnPointManager.InitializeSpawnPoint(spawnPoints);
    //}

    //protected override void Update()
    //{
    //    spawnPointManager.UpdateCooldowns();
    //    base.Update();
    //}

    protected override void SpawnObject()
    {
        //var availablePoints = spawnPoints.Where(point => 
        //spawnPointManager.IsPointAvailable(point, checkRadius, typeof(MedKit))).ToList();

        //if (!CheckerToNull.CheckArrayNotEmpty(spawnPoints, nameof(spawnPoints)) || availablePoints.Count == 0)
        //{
        //    //Debug.LogWarning("MedkitSpawner: Точки спавна недоступны или нет свободных точек для спавна.");
        //    return;
        //}

        //Transform spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];

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
