using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsController : MonoBehaviour
{
    [SerializeField] public BulletsData bulletData;

    private ObjectPool objectPool;

    private void Update()
    {
        MoveBullet(bulletData);
    }

    public void SetPool(ObjectPool pool)
    {
        objectPool = pool;
        Debug.Log($"BulletsController: {gameObject.name} привязан к пулу {pool.gameObject.name}");
    }

    private void MoveBullet(BulletsData bulletData)
    {
        transform.position = Vector3.MoveTowards(transform.position, bulletData.Target, bulletData.Speed * Time.deltaTime);
        if (!bulletData.HitTarget && Vector3.Distance(transform.position, bulletData.Target) < bulletData.DistanceToTarget)
        {
            bulletData.HitTarget = true;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log($"BulletsController: {gameObject.name} столкнулась с {other.name}");
    //    if (other.CompareTag("Enemy") || other.CompareTag("Environment"))
    //    {
    //        //ObjectPool pool = FindObjectOfType<ObjectPool>();
    //        if (/*pool*/objectPool != null)
    //        {
    //            Debug.Log($"BulletsController: Деспавн {gameObject.name} через пул {objectPool.gameObject.name}");
    //            /*pool*/objectPool.Despawn(gameObject);
    //        }
    //        else
    //        {
    //            Debug.LogError($"BulletsController: ObjectPool не назначен для {gameObject.name}!");
    //        }
    //    }
    //}


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Environment"))
        {
            if (objectPool != null)
            {
                Debug.Log($"BulletsController: {gameObject.name} возвращена в пул {objectPool.gameObject.name}");
                DespawnBullet();
            }
            else
            {
                Debug.LogError($"BulletsController: ObjectPool Не назначен для {gameObject.name}!");
            }
        }
    }

    private void DespawnBullet()
    {
        Debug.Log($"BulletsController: Попытка деспавна {gameObject.name}");

        if (objectPool == null)
        {
            Debug.LogError($"BulletsController: ObjectPool не найден при попытке деспавна {gameObject.name}!");
            return;
        }
        gameObject.SetActive(false);
        objectPool.Despawn(gameObject);
        Debug.Log($"BulletsController: {gameObject.name} успешно деспавнена в {objectPool.gameObject.name}");
    }
}

