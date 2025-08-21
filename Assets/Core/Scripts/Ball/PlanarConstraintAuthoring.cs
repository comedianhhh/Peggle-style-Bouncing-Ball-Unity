using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public class PlanarConstraintAuthoring : MonoBehaviour
{
    public float fixedZPosition = 0f;
    public bool constrainVelocity = true;

    public class Baker : Baker<PlanarConstraintAuthoring>
    {
        public override void Bake(PlanarConstraintAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlanarConstraint
            {
                fixedZPosition = authoring.fixedZPosition,
                constrainVelocity = authoring.constrainVelocity
            });
        }
    }
}
public struct PlanarConstraint : IComponentData
{
    public float fixedZPosition;
    public bool constrainVelocity;
}