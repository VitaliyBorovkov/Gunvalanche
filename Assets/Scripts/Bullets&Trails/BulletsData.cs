using System;
using UnityEngine;

[Serializable]
public struct BulletsData
{
    public GameObject BulletPrefab;
    
    public string Name;

    public float Speed;
    public float DistanceToTarget;
    public float LifeTime;

    public bool HitTarget { get; set; }

    public Vector3 Target { get; set; }

}
