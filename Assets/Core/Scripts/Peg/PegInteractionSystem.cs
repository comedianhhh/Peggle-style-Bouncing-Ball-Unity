using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(BallPegCollisionSystem))]
[BurstCompile]
public partial struct PegInteractionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<GameSettings>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var gameSettings = SystemAPI.GetSingletonRW<GameSettings>();

        foreach (var (peg, entity) in
                 SystemAPI.Query<RefRW<Peg>>()
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
                    // Placeholder for bonus ball creation
                    ecb.DestroyEntity(entity);
                    break;
            }
        }
    }
}
