using System;

using UnityEngine;

[Serializable]
public struct BulletsData
{
    public string Name;

    public float Speed;
    public float DistanceToTarget;
    public float LifeTime;

    public GameObject BulletPrefab;
    public GameObject ExplosionEffectPrefab;
    public BulletsType BulletsType;

    public bool HitTarget { get; set; }

    public Vector3 Target { get; set; }
}
