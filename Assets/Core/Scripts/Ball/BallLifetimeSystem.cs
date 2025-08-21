using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BallLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameSettingsEntity = SystemAPI.GetSingletonEntity<GameSettings>();
        var gameSettings = SystemAPI.GetComponentRW<GameSettings>(gameSettingsEntity);
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        if (gameSettings.ValueRO.gameState == GameState.Playing || gameSettings.ValueRO.gameState == GameState.Bonus)
        {
            foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalToWorld>>().WithEntityAccess())
            {
                if (transform.ValueRO.Position.y < -20f)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
    

}
