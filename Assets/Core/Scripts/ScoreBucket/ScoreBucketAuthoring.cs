using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class ScoreBucketAuthoring : MonoBehaviour
{
    public int scoreValue = 100;
    public class ScoreBucketAuthoringBaker : Baker<ScoreBucketAuthoring>
    {
        public override void Bake(ScoreBucketAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ScoreBucketTag());
        }
    }
}

