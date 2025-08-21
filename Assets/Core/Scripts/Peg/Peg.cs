using Unity.Entities;

[System.Serializable]
public struct Peg : IComponentData
{
    public PegType Type;
    public int PointValue;
    public bool IsHit;
    public int bonusBallAmount;
}