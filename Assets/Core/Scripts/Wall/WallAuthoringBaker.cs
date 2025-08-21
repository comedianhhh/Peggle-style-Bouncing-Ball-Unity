using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class WallAuthoringBaker : Baker<WallAuthoring>
{
    public override void Bake(WallAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new StaticTag());
    }
}
