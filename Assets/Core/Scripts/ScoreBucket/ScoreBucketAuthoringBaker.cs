using Unity.Entities;
using UnityEngine;

public class ScoreBucketAuthoringBaker : Baker<ScoreBucketAuthoring>
{
    public override void Bake(ScoreBucketAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ScoreBucketTag());
    }
}
