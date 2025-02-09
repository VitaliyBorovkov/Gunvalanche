using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject ObjectPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> ObjectQueue;
    private int activeObjectsCount = 0;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        ObjectQueue = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ObjectPrefab);
            ObjectQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        if (ObjectQueue.Count == 0)
        {
            Debug.LogWarning($"ObjectPool: {gameObject.name} ����, ��������� ���.");
            ExpandPool();
        }

        GameObject obj = ObjectQueue.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        activeObjectsCount++;

        //Debug.Log($"ObjectPool: {gameObject.name} - ����� {obj.name}. ��������: {activeObjectsCount}");
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        Debug.Log($"ObjectPool: ���������� ����� Despawn");

        if (obj == null)
        {
            Debug.LogError("ObjectPool: ������� �������� null �������!");
            return;
        }
        Debug.Log($"[ObjectPool] ������ Despawn() ��� {obj.name}");
        obj.SetActive(false);
        //Debug.Log($"ObjectPool: {gameObject.name} - {obj.name} ��������");
        obj.transform.position = transform.position;
        obj.transform.rotation = Quaternion.identity;
        ObjectQueue.Enqueue(obj);
        //Debug.Log($"ObjectPool: {gameObject.name} - {obj.name} �������� ������� � �������.");
        activeObjectsCount--;

        //Debug.Log($"ObjectPool: {gameObject.name} - ������� {obj.name}. ������� ����: {ObjectQueue.Count}, ��������: {activeObjectsCount}");
    }

    public int CountActiveObjects()
    {
        return activeObjectsCount;
    }

    private void ExpandPool()
    {
        int currentPoolSize = ObjectQueue.Count;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ObjectPrefab);
            ObjectQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }
}