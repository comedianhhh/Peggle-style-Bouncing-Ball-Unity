using Unity.Entities;
using Unity.Mathematics;

public struct LauncherInputData : IComponentData
{
    public bool launch;
    public float launchForce;
    public float3 launchDirection;
}
