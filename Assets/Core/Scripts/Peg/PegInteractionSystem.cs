using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(BallBounceSystem))]
[BurstCompile]
public partial struct PegInteractionSystem : ISystem
{
    private Random _random;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<GameSettings>();
        _random = Random.CreateFromIndex(1234);
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var gameSettings = SystemAPI.GetSingletonRW<GameSettings>();
        
        bool canSpawnBonusBalls = SystemAPI.HasSingleton<BonusBallPrefabs>();
        DynamicBuffer<BonusBallPrefabs> bonusBallPrefabs = default;
        if (canSpawnBonusBalls)
        {
            bonusBallPrefabs = SystemAPI.GetSingletonBuffer<BonusBallPrefabs>();
            if (bonusBallPrefabs.IsEmpty)
            {
                canSpawnBonusBalls = false;
            }
        }

        foreach (var (peg, ltw, entity) in
                 SystemAPI.Query<RefRW<Peg>, RefRO<LocalToWorld>>()
                          .WithAll<PegTag>()
                          .WithEntityAccess())
        {
            if (!peg.ValueRO.IsHit)
                continue;

            switch (peg.ValueRO.Type)
            {
                case PegType.Red:
                    gameSettings.ValueRW.redPegsRemaining--;
                    gameSettings.ValueRW.currentScore += peg.ValueRO.PointValue;
                    ecb.DestroyEntity(entity);
                    break;

                case PegType.Blue:
                    gameSettings.ValueRW.currentScore += peg.ValueRO.PointValue;
                    ecb.DestroyEntity(entity);
                    break;

                case PegType.Green:
                    gameSettings.ValueRW.currentScore += peg.ValueRO.PointValue;
                    if (canSpawnBonusBalls)
                    {
                        for (int i = 0; i < peg.ValueRO.bonusBallAmount; i++)
                        {
                            var bonusBall = ecb.Instantiate(bonusBallPrefabs[0].Value);
                            ecb.SetComponent(bonusBall, new LocalTransform
                            {
                                Position = ltw.ValueRO.Position,
                                Rotation = quaternion.identity,
                                Scale = 0.5f
                            });

                            var randomVelocity = _random.NextFloat2Direction() * 5f;
                            ecb.SetComponent(bonusBall, new PhysicsVelocity
                            {
                                Linear = new float3(randomVelocity.x, math.abs(randomVelocity.y), 0),
                                Angular = float3.zero
                            });

                            ecb.SetComponent(bonusBall, new BallState { Value = BallStateValue.InPlay });
                        }
                    }
                    ecb.DestroyEntity(entity);
                    break;
            }
        }
    }
}
