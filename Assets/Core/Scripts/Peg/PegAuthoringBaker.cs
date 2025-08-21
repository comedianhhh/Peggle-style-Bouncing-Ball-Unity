using Unity.Entities;

public class PegAuthoringBaker : Baker<PegAuthoring>
{
    public override void Bake(PegAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Renderable);
        
        AddComponent(entity, new PegTag());
        AddComponent(entity, new Peg
        {
            Type = authoring.pegType,
            PointValue = authoring.pointValue,
            IsHit = false
        });
    }
}