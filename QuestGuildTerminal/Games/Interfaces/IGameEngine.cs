public interface IGameEngine
{
    string GameName { get; }
    Task<GameResult> StartGameAsync(int targetLevel);
    event Action<int> OnLevelUp;  // Notify when level changes
    event Action<bool> OnGameComplete;
}

public class GameResult
{
    public bool Success { get; set; }
    public int FinalLevel { get; set; }
    public int Score { get; set; }
    public TimeSpan TimePlayed { get; set; }
}