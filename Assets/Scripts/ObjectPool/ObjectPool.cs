using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject ObjectPrefab;
    [SerializeField] private int PoolSize = 10;

    private Queue<GameObject> ObjectQueue;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        ObjectQueue = new Queue<GameObject>();

        for (int i = 0; i < PoolSize; i++)
        {
            GameObject obj = Instantiate(ObjectPrefab);
            obj.SetActive(false);
            ObjectQueue.Enqueue(obj);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) 
    {
        if (ObjectQueue.Count == 0)
        {
            ExpandPool();
        }

        GameObject obj = ObjectQueue.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnSpawn();
        }
        
        return obj;
    }

    public void Despawn(GameObject obj, float delay)
    {
        StartCoroutine(DespawnWithDelay(obj, delay));
    }

    private IEnumerator DespawnWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(false);

        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnDespawn();
        }

        ObjectQueue.Enqueue(obj);
    }

    private void ExpandPool()
    {
        int currentPoolSize = ObjectQueue.Count;

        for (int i = 0; i < PoolSize - currentPoolSize; i++)
        {
            GameObject obj = Instantiate(ObjectPrefab);
            obj.SetActive(false);
            ObjectQueue.Enqueue(obj);
        }
    }
}
