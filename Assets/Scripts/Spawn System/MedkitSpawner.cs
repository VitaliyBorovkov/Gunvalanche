using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MedkitSpawner : ObjectSpawner
{
    [Header("Medkit Settings")]
    [SerializeField] private GameObject medkitPrefab;
    [SerializeField] private float checkRadius = 0.5f;

    private SpawnPointManager spawnPointManager;

    private void Start()
    {
        spawnPointManager = FindObjectOfType<SpawnPointManager>();

        if (spawnPointManager == null)
        {
            Debug.Log("MedkitSpawner: SpawnPointManager не найден на сцене!");
            enabled = false;
            return;
        }

        spawnPointManager.InitializeSpawnPoint(spawnPoints);
    }

    protected override void Update()
    {
        spawnPointManager.UpdateCooldowns();
        base.Update();
    }

    protected override void SpawnObject()
    {
        var availablePoints = spawnPoints.Where(point => 
        spawnPointManager.IsPointAvailable(point, checkRadius, typeof(MedKit))).ToList();

        if (availablePoints.Count == 0)
        {
            Debug.Log("MedkitSpawner: Нет доступных точек для спавна аптечек.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(medkitPrefab, spawnPoint.position, Quaternion.identity);
        spawnPointManager.SetCooldown(spawnPoint);
    }

    protected override int CountActiveObjects()
    {
        /*ICollectible[] collevtibles =*/return GameObject.FindObjectsOfType<MonoBehaviour>().
            OfType<ICollectible>().Where(item => item is MedKit).ToArray().Length;

        //return collevtibles.Length;
    }
}
