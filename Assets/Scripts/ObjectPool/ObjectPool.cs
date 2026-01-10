using System.Collections.Generic;

using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private const string LOG_PREFIX = "ObjectPool";

    [SerializeField] private GameObject ObjectPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> ObjectQueue;
    private int activeObjectsCount = 0;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (ObjectQueue != null)
        {
            return;
        }

        ObjectQueue = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ObjectPrefab);
            ObjectQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation, bool autoActivate = true)
    {
        if (ObjectQueue.Count == 0)
        {
            Debug.LogWarning($"{LOG_PREFIX}: {gameObject.name} is empty, expanced pool.");
            ExpandPool();
        }

        GameObject obj = ObjectQueue.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        if (autoActivate)
        {
            obj.SetActive(true);
        }


        activeObjectsCount++;

        //Debug.Log($"{LOG_PREFIX}: {gameObject.name} - Spawn {obj.name}. Active: {activeObjectsCount}");
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        //Debug.Log($"{LOG_PREFIX}: Called method Despawn");

        if (obj == null)
        {
            Debug.LogError($"{LOG_PREFIX}: Trying to despawn 'null' object!");
            return;
        }
        //Debug.Log($"{LOG_PREFIX}: Called Despawn() for {obj.name}");
        obj.SetActive(false);
        //Debug.Log($"{LOG_PREFIX}: {gameObject.name} - {obj.name} turn off");
        obj.transform.position = transform.position;
        obj.transform.rotation = Quaternion.identity;
        ObjectQueue.Enqueue(obj);
        //Debug.Log($"{LOG_PREFIX}: {gameObject.name} - {obj.name} returned to pool.");
        activeObjectsCount--;

        //Debug.Log($"{LOG_PREFIX}: {gameObject.name} - Despawn {obj.name}. Queue pool: {ObjectQueue.Count}, Active: {activeObjectsCount}");
    }

    public int CountActiveObjects()
    {
        return activeObjectsCount;
    }

    private void ExpandPool()
    {
        //int currentPoolSize = ObjectQueue.Count;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ObjectPrefab);
            ObjectQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }
}