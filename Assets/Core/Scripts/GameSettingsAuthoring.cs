using UnityEngine;
using Unity.Entities;

public class GameSettingsAuthoring : MonoBehaviour
{
    public int startingScore = 0;
    public class GameSettingsBaker : Baker<GameSettingsAuthoring>
    {
        public override void Bake(GameSettingsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameSettings
            {
                currentScore = authoring.startingScore,
                redPegsRemaining = 0 // This will be calculated at the start of a level
            });
        }
    }
}