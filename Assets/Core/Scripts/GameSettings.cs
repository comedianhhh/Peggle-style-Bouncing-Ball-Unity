using Unity.Entities;

public enum GameState
{
    Playing,
    Bonus,
    Won,
    Lost
}

public struct GameSettings : IComponentData
{
    public float bonusTimer;
    public int currentScore;
    public int ballsRemaining;
    public int redPegsRemaining;
    public GameState gameState;
}