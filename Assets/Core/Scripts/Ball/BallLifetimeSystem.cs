using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct BallLifetimeSystem : ISystem
{
    private const float OutOfBoundsY = -10f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var gameSettings = SystemAPI.GetSingletonRW<GameSettings>();
        
        // Destroy balls that go out of bounds
        foreach (var (transform, entity) in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<BallTag>()
                     .WithNone<ScoreBucketHit>()
                     .WithEntityAccess())
        {
            if (transform.ValueRO.Position.y < OutOfBoundsY)
            {
                ecb.DestroyEntity(entity);
            }
        }
        
        foreach (var (scoreValue, entity) in 
                 SystemAPI.Query<RefRO<BallScoreValue>>()
                     .WithAll<BallTag, ScoreBucketHit>()
                     .WithEntityAccess())
        {
            gameSettings.ValueRW.currentScore += scoreValue.ValueRO.Value;
            gameSettings.ValueRW.ballsRemaining++;
            ecb.DestroyEntity(entity);
        }
        
        ecb.Playback(state.EntityManager);
    }
}
