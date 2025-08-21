using Unity.Entities;

public struct GameSettings : IComponentData
{
    public int currentScore;
    public int ballsRemaining;
    public int redPegsRemaining;

}