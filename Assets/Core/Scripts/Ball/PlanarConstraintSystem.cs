
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct PlanarConstraintSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, constraint) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlanarConstraint>>())
        {
            var pos = transform.ValueRO.Position;
            if (math.abs(pos.z - constraint.ValueRO.fixedZPosition) > 0.001f)
            {
                pos.z = constraint.ValueRO.fixedZPosition;
                transform.ValueRW.Position = pos;
            }
        }

        if (SystemAPI.HasSingleton<PhysicsStep>())
        {
            foreach (var (velocity, constraint) in 
                     SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<PlanarConstraint>>())
            {
                if (constraint.ValueRO.constrainVelocity)
                {
                    var vel = velocity.ValueRO.Linear;
                    vel.z = 0f; 
                    velocity.ValueRW.Linear = vel;
                    
                    var angVel = velocity.ValueRO.Angular;
                    angVel.x = 0f; 
                    angVel.y = 0f; 
                    velocity.ValueRW.Angular = angVel;
                }
            }
        }
    }
}
