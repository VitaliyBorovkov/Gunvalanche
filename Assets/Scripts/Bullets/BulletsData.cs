using System;
using UnityEngine;

[Serializable]
public struct BulletsData
{
    public string Name;

    public float Speed;
    public float DistanceToTarget;

    public GameObject BulletPrefab;
    public BulletsType BulletsType;

    public bool HitTarget { get; set; }

    public Vector3 Target { get; set; }
}
