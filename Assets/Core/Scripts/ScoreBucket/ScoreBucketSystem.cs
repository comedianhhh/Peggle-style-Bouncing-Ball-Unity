using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ScoreBucketSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<GameSettings>();
    }
    
    

    public void OnUpdate(ref SystemState state)
    {
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();

        // Enable/disable buckets based on game state
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (bucket, entity) in SystemAPI.Query<ScoreBucketTag>().WithEntityAccess())
        {
            if (gameSettings.gameState == GameState.Bonus)
            {
                //  Enable rendering
                if (SystemAPI.HasComponent<DisableRendering>(entity))
                    ecb.RemoveComponent<DisableRendering>(entity);
            }
            else
            {
                // Disable rendering
                if (!SystemAPI.HasComponent<DisableRendering>(entity))
                    ecb.AddComponent<DisableRendering>(entity);
            }
        }
        ecb.Playback(state.EntityManager);

        // Only run trigger job logic in Bonus state
        if (gameSettings.gameState != GameState.Bonus)
        {
            return;
        }
        
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var jobEcb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new TriggerJob
        {
            ECB = jobEcb.AsParallelWriter(),
            BallLookup = SystemAPI.GetComponentLookup<BallTag>(true),
            BucketLookup = SystemAPI.GetComponentLookup<ScoreBucketTag>(true)
        };

        var sim = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = job.Schedule(sim, state.Dependency);
    }

    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ComponentLookup<BallTag> BallLookup;
        [ReadOnly] public ComponentLookup<ScoreBucketTag> BucketLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            var isABall = BallLookup.HasComponent(entityA);
            var isBBall = BallLookup.HasComponent(entityB);
            var isAScoreBucket = BucketLookup.HasComponent(entityA);
            var isBScoreBucket = BucketLookup.HasComponent(entityB);

            if (isABall && isBScoreBucket)
            {
                ECB.DestroyEntity(0, entityA);
                ECB.AddComponent(0, entityB, new ScoreBucketHit());
            }
            else if (isBBall && isAScoreBucket)
            {
                ECB.DestroyEntity(0, entityB);
                ECB.AddComponent(0, entityA, new ScoreBucketHit());
            }
        }
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(ScoreBucketSystem))]
public partial struct ScoreBucketHitResolutionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var gameSettings = SystemAPI.GetSingletonRW<GameSettings>();
        
        foreach(var (bucket, hit, entity) in SystemAPI.Query<ScoreBucketTag, ScoreBucketHit>().WithEntityAccess())
        {
            gameSettings.ValueRW.currentScore += bucket.scoreValue;
            ecb.RemoveComponent<ScoreBucketHit>(entity);
        }
        
        ecb.Playback(state.EntityManager);
    }
}

