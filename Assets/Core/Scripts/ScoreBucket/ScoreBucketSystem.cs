using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ScoreBucketSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state) 
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new TriggerJob
        {
            ECB = ecb.AsParallelWriter(),
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
            }
            else if (isBBall && isAScoreBucket)
            {
                ECB.DestroyEntity(0, entityB);
            }
        }
    }
}