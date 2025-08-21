using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

[BurstCompile]
public partial struct GameStateSystem : ISystem
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

        switch (gameSettings.ValueRO.gameState)
        {
            case GameState.Playing:
                if (gameSettings.ValueRO.redPegsRemaining == 0)
                {
                    gameSettings.ValueRW.gameState = GameState.Bonus;
                    UnityEngine.Debug.LogWarning("Switched state -> Bonus");
                }
                else if (gameSettings.ValueRO.ballsRemaining == 0)
                {
                    var ballQuery = SystemAPI.QueryBuilder().WithAll<BallTag, BallState>().Build();
                    if (ballQuery.IsEmpty)
                    {
                        gameSettings.ValueRW.gameState = GameState.Lost;
                        UnityEngine.Debug.LogWarning("Switched state -> Lost");
                    }
                }
                break;
            case GameState.Bonus:
                
                var ballsInPlayQuery = SystemAPI.QueryBuilder().WithAll<BallTag, BallState>().Build();
                if (ballsInPlayQuery.IsEmpty)
                {
                    gameSettings.ValueRW.gameState = GameState.Won;
                    UnityEngine.Debug.LogWarning("Switched state -> Won");
                }
                break;
            case GameState.Won:
                break;
            case GameState.Lost:
                break;
        }
    }
}
