using Unity.Entities;
using UnityEngine;

public class BouncyAuthoring : MonoBehaviour
{
    [Range(0,1.5f)]
    public float Restitution = 0.8f;

    class BouncyBaker : Baker<BouncyAuthoring>
    {
        public override void Bake(BouncyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bouncy { Restitution = authoring.Restitution });
        }
    }
}
