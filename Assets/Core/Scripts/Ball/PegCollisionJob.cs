using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public struct PegCollisionJob : ICollisionEventsJob
{
    [ReadOnly] public ComponentLookup<BallTag> BallLookup;
    public ComponentLookup<Peg> PegLookup;
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute(CollisionEvent collisionEvent)
    {
        var entityA = collisionEvent.EntityA;
        var entityB = collisionEvent.EntityB;

        bool isABall = BallLookup.HasComponent(entityA);
        bool isBBall = BallLookup.HasComponent(entityB);
        bool isAPeg = PegLookup.HasComponent(entityA);
        bool isBPeg = PegLookup.HasComponent(entityB);

        if ((isABall && isBPeg) || (isBBall && isAPeg))
        {
            var pegEntity = isAPeg ? entityA : entityB;
            if (PegLookup.HasComponent(pegEntity) && !PegLookup[pegEntity].IsHit)
            {
                var peg = PegLookup[pegEntity];
                peg.IsHit = true;
                ECB.SetComponent(pegEntity.Index, pegEntity, peg);
            }
        }
    }
}