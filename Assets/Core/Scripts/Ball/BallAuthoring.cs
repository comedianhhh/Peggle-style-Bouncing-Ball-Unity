using Unity.Entities;
using UnityEngine;

public class BallAuthoring : MonoBehaviour
{
    public int scoreValue = 10;
    
    
    public class BallAuthoringBaker : Baker<BallAuthoring>
    {
        public override void Bake(BallAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BallTag());
            AddComponent(entity, new BallState { Value = BallStateValue.ReadyToLaunch });
        }
    }
}
