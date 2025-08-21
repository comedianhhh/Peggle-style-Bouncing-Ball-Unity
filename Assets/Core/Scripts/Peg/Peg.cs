using Unity.Entities;

public struct Peg : IComponentData
{
    public PegType Type;
    public int PointValue;
    public bool IsHit;
}