using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsController : MonoBehaviour
{
    [SerializeField] public BulletsData bulletData;

    private void Update()
    {
        MoveBullet(bulletData);
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
