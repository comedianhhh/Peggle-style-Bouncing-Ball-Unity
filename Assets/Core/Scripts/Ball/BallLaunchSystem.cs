using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct BallLaunchSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (spawner, inputData, localToWorld) in SystemAPI.Query<RefRO<BallSpawner>, RefRO<LauncherInputData>, RefRO<LocalToWorld>>())
        {
            if (!inputData.ValueRO.launch)
            {
                continue;
            }
            
            var ballEntity = ecb.Instantiate(spawner.ValueRO.BallPrefab);

            var launcherPosition = localToWorld.ValueRO.Position;
            var launchDirection = inputData.ValueRO.launchDirection;
            var launchForce = inputData.ValueRO.launchForce;

            ecb.SetComponent(ballEntity, new LocalTransform
            {
                Position = launcherPosition,
                Rotation = quaternion.identity,
                Scale = 1
            });
            
            var impulse = launchDirection * launchForce;
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
