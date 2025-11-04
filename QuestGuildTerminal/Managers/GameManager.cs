using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuestGuildTerminal.Services;
using QuestGuildTerminal.Games.Tetris;

namespace QuestGuildTerminal  // ADD NAMESPACE
{
    public class GameManager
    {
        private readonly Dictionary<string, IGameEngine> _games;
        private readonly IMusicService _musicService; // Now can use simple name due to using statement
        
        public GameManager(IMusicService musicService = null)
        {
            _musicService = musicService;
            _games = new Dictionary<string, IGameEngine>
            {
                ["tetris"] = new TetrisEngine(musicService)
            };
            
            Console.WriteLine("🎮 GameManager initialized with Tetris");
            Console.WriteLine($"🎵 Music service: {(musicService != null ? "AVAILABLE" : "NOT AVAILABLE")}");
        }
        
        public GameManager() : this(null)
        {
        }
        
        public async Task<GameResult> PlayGameAsync(string gameName, int targetLevel)
        {
            Console.WriteLine($"🎮 Attempting to play {gameName} with target level {targetLevel}");
            
            if (_games.TryGetValue(gameName.ToLower(), out var game))
            {
                Console.WriteLine($"🎮 Found game: {game.GameName}");
                Console.WriteLine("🎮 Starting game...");
                
                var result = await game.StartGameAsync(targetLevel);
                
                Console.WriteLine($"🎮 Game completed: Success={result.Success}, Level={result.FinalLevel}");
                return result;
            }
            
            Console.WriteLine($"❌ Game '{gameName}' not found!");
            throw new ArgumentException($"Game '{gameName}' not found");
        }
        
        public List<string> GetAvailableGames()
        {
            return new List<string>(_games.Keys);
        }
    }
}