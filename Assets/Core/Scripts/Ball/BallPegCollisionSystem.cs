using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct BallPegCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<BallTag>();
        state.RequireForUpdate<Peg>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var sim = SystemAPI.GetSingleton<SimulationSingleton>();
        var job = new PegCollisionJob
        {
            BallLookup = SystemAPI.GetComponentLookup<BallTag>(true),
            PegLookup = SystemAPI.GetComponentLookup<Peg>(false),
            ECB = ecb.AsParallelWriter()
        };

        state.Dependency = job.Schedule(sim, state.Dependency);
    }
}