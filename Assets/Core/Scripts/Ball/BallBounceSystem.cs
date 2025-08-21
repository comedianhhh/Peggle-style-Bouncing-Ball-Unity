using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct BallBounceSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<BallTag>();
        state.RequireForUpdate<Bouncy>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var sim = SystemAPI.GetSingleton<SimulationSingleton>();
        var job = new BounceCollisionJob
        {
            BallLookup = SystemAPI.GetComponentLookup<BallTag>(true),
            BouncyLookup = SystemAPI.GetComponentLookup< Bouncy>(true),
            VelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(false),
            PegLookup = SystemAPI.GetComponentLookup<Peg>(false),
            ECB = ecb.AsParallelWriter()
        };

        state.Dependency = job.Schedule(sim, state.Dependency);
    }
}

[BurstCompile]
public struct BounceCollisionJob : ICollisionEventsJob
{
    [ReadOnly] public ComponentLookup<BallTag> BallLookup;
    [ReadOnly] public ComponentLookup<Bouncy> BouncyLookup;
    public ComponentLookup<PhysicsVelocity> VelocityLookup;
    public ComponentLookup<Peg> PegLookup;
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute(CollisionEvent collisionEvent)
    {
        var entityA = collisionEvent.EntityA;
        var entityB = collisionEvent.EntityB;

        bool isABall = BallLookup.HasComponent(entityA);
        bool isBBall = BallLookup.HasComponent(entityB);
        bool isABouncy = BouncyLookup.HasComponent(entityA);
        bool isBBouncy = BouncyLookup.HasComponent(entityB);

        if ((isABall && isBBouncy) || (isBBall && isABouncy))
        {
            var bouncyEntity = isABouncy ? entityA : entityB;
            var ballEntity = isABall ? entityA : entityB;

            // Custom bounce logic
            var velocity = VelocityLookup[ballEntity];
            var collisionNormal = collisionEvent.Normal;
            
            var reflectedVelocity = velocity.Linear - 2 * math.dot(velocity.Linear, collisionNormal) * collisionNormal;
            
            var bouncy = BouncyLookup[bouncyEntity];
            velocity.Linear = reflectedVelocity * bouncy.Restitution;
            
            VelocityLookup[ballEntity] = velocity;
            
            // Peg-specific logic
            if (PegLookup.HasComponent(bouncyEntity))
            {
                var peg = PegLookup[bouncyEntity];
                if (!peg.IsHit)
                {
                    peg.IsHit = true;
                    ECB.SetComponent(bouncyEntity.Index, bouncyEntity, peg);
                }
            }
        }
    }
}
