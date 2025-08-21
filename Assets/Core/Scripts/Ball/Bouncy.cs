using Unity.Entities;
using UnityEngine;

public struct Bouncy : IComponentData
{
    [Range(0,1.5f)]
    public float Restitution;
}
