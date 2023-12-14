using System;
using UnityEngine;

[Serializable]
public struct EntityData
{
    public string Name;
    
    public int Health;

    public float WalkSpeed;
    public float RunSpeed;

    public GameObject Prefab;
}
