using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct PurplePegPositionBuffer : IBufferElementData
{
    public float3 Value;
}
