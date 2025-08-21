using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;




[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))] // ensures LocalTransform exists before transforms update
[BurstCompile]
public partial struct BallLaunchSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (spawner, inputData, localToWorld) in
                 SystemAPI.Query<RefRO<BallSpawner>, RefRO<LauncherInputData>, RefRO<LocalToWorld>>())
        {
            if (!inputData.ValueRO.launch)
            {
                continue;
            }
            
            var ballEntity = ecb.Instantiate(spawner.ValueRO.BallPrefab);

            ecb.SetComponent(ballEntity, new LocalTransform
            {
                Position = localToWorld.ValueRO.Position,
                Rotation = quaternion.identity,
                Scale = 0.5f
            });
            
            var impulse = inputData.ValueRO.launchDirection * inputData.ValueRO.launchForce;
            ecb.SetComponent(ballEntity, new PhysicsVelocity
            {
                Linear = impulse,
                Angular = float3.zero
            });
            
            ecb.SetComponent(ballEntity, new BallState { Value = BallStateValue.InPlay });
        }
        ecb.Playback(state.EntityManager);
    }
}
