using Unity.Entities;
using UnityEngine;

public class BrickAuthoring : MonoBehaviour
{
    class BrickBaker : Baker<BrickAuthoring>
    {
        public override void Bake(BrickAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BrickTag>(entity);
        }
    }
}
