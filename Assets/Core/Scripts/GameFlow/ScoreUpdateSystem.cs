using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public partial struct ScoreUpdateSystem : ISystem
{
    private bool runOnce;
    
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
        if (runOnce)
        {
            state.Enabled = false;
            return;
        }

        // Count red pegs that are NOT hit
        var redPegCount = 0;
        
        foreach (var peg in SystemAPI.Query<RefRO<Peg>>())
        {
            if (peg.ValueRO.Type == PegType.Red)
                redPegCount++;
        }
        

        var gameSettingsEntity = SystemAPI.GetSingletonEntity<GameSettings>();
        var gameSettings = SystemAPI.GetComponentRW<GameSettings>(gameSettingsEntity);
        gameSettings.ValueRW.redPegsRemaining = redPegCount;
        runOnce = true;
    }
}
