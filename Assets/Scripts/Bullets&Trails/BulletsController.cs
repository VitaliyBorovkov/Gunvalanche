using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletsController : MonoBehaviour
{
    [SerializeField] public BulletsData bulletData;

    private void Update()
    {
        if (IsBulletDataInvalid())
        {
            Debug.LogError("BulletsController. BulletData is invalid or not initialized properly.");
            return;
        }


        MoveBullet(bulletData);
    }

    private bool IsBulletDataInvalid()
    {
        // Check if bulletData has default values to determine if it's uninitialized
        return bulletData.Target == Vector3.zero && bulletData.Speed == 0;
    }

    private void MoveBullet(BulletsData bulletData)
    {
        transform.position = Vector3.MoveTowards(transform.position, bulletData.Target, bulletData.Speed * Time.deltaTime);
        if (!bulletData.HitTarget && Vector3.Distance(transform.position, bulletData.Target) < bulletData.DistanceToTarget)
        {
            bulletData.HitTarget = true;
            Debug.Log(" BulletsController. Bullet reached target position.");
        }
    }
}
