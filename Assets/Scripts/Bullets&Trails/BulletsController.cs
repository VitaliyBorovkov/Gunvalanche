using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletsController : MonoBehaviour
{
    [SerializeField] public BulletsData[] bulletsData;

    private void Update()
    {
        if (bulletsData != null && bulletsData.Length > 0)
        {
            foreach (var bulletData in bulletsData)
            {
                MoveBullet(bulletData);
            }
        }
    }

    private void MoveBullet(BulletsData bulletData)
    {
        transform.position = Vector3.MoveTowards(transform.position, bulletData.Target, bulletData.Speed * Time.deltaTime);
        if (!bulletData.HitTarget && Vector3.Distance(transform.position, bulletData.Target) < bulletData.DistanceToTarget)
        {
            bulletData.HitTarget = true;
        }
    }
}
