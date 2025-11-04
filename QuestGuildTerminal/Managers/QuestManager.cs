using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestGuildTerminal.Games.Tetris;
using QuestGuildTerminal.Services;

namespace QuestGuildTerminal
{
    public class QuestManager : IQuestManager 
    {
        private List<Quest> _quests;
        private readonly GameManager _gameManager;
         private readonly Dictionary<string, IGameEngine> _games;
         private readonly IMusicService _musicService;
        public QuestManager(IMusicService musicService = null)
    {
            _quests = new List<Quest>();
        _musicService = musicService;
        _games = new Dictionary<string, IGameEngine>
        {
            ["tetris"] = new TetrisEngine(musicService) // Pass music service to Tetris
        };
    }
    
    public async Task<GameResult> PlayGameAsync(string gameName, int targetLevel)
    {
        if (_games.TryGetValue(gameName.ToLower(), out var game))
        {
            return await game.StartGameAsync(targetLevel);
        }
        throw new ArgumentException($"Game '{gameName}' not found");
    }
    
    public List<string> GetAvailableGames()
    {
        return _games.Keys.ToList();
    }


        // Add GameManager dependency
        public QuestManager(GameManager gameManager = null)
        {
            _quests = new List<Quest>();
            _gameManager = gameManager;
        }

        // Implement interface method and forward to overload
        public void AddQuest(Quest quest)
        {
            // forward to overload that supports optional game challenge
            AddQuest(quest, includeGameChallenge: true);
        }

        // Add quest with optional game challenge
        public void AddQuest(Quest quest, bool includeGameChallenge = true)
        {
            if (includeGameChallenge && _gameManager != null)
            {
                quest.RequiresGameCompletion = true;
                quest.RequiredGame = "Tetris";
                quest.RequiredGameLevel = 3;
            }
            
            _quests.Add(quest);
        }

        // NEW METHOD Attempt quest completion with game challenge
        public async Task<bool> AttemptQuestCompletionAsync(string id)
        {
            var quest = GetQuestById(id);
            if (quest == null || quest.IsCompleted)
                return false;

            // If no game requirement, complete immediately
            if (!quest.RequiresGameCompletion || _gameManager == null)
            {
                quest.MarkComplete();
                return true;
            }

            Console.WriteLine($"\n🎮 This quest requires completing {quest.RequiredGame} Level {quest.RequiredGameLevel}");
            Console.WriteLine("You must prove your skills in Tetris!");
            Console.Write("Start the game challenge? (y/n): ");
            var choice = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (char.ToLower(choice) == 'y')
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("🎮 Launching Tetris Challenge...");
                    Console.WriteLine($"🎯 Objective: Reach Level {quest.RequiredGameLevel}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();

                    var result = await _gameManager.PlayGameAsync(
                        quest.RequiredGame, 
                        quest.RequiredGameLevel
                    );

                    if (result.Success)
                    {
                        quest.MarkGameCompleted();
                        Console.WriteLine($"\n🎉 Challenge Completed! Reached Level {result.FinalLevel}");
                        Console.WriteLine($"🏆 Score: {result.Score}");
                        Console.WriteLine("Your quest is now ready for completion!");
                        
                        // Auto-complete after game success
                        quest.MarkComplete();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"\n❌ Challenge Failed! Only reached Level {result.FinalLevel}");
                        Console.WriteLine("Keep practicing and try again!");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n⚠️ Game error: {ex.Message}");
                    Console.WriteLine("Completing quest without game challenge.");
                    quest.MarkComplete();
                    return true;
                }
            }

            return false;
        }

        //  CompleteQuest now handles game challenges
        public bool CompleteQuest(string id)
        {
            var quest = GetQuestById(id);
            if (quest != null && !quest.IsCompleted)
            {
                // If game is required but not completed
                if (quest.RequiresGameCompletion && !quest.IsGameCompleted)
                {
                    Console.WriteLine("❌ Cannot complete quest! You must first complete the game challenge.");
                    Console.WriteLine("Use 'Attempt Quest Completion' to play the game.");
                    return false;
                }

                quest.MarkComplete();
                return true;
            }
            return false;
        }

        // NEW METHOD: Get quests that need game completion
        public List<Quest> GetQuestsWithGameChallenge()
        {
            return _quests.Where(q => q.RequiresGameCompletion && !q.IsGameCompleted).ToList();
        }

        // NEW METHOD: Check if quest has pending game challenge
        public bool HasPendingGameChallenge(string id)
        {
            var quest = GetQuestById(id);
            return quest?.RequiresGameCompletion == true && !quest.IsGameCompleted;
        }

        //  Update quest summary to show game challenges
        public string GetQuestSummary()
        {
            var activeQuests = GetActiveQuests();
            var completedQuests = GetCompletedQuests();
            var nearDeadline = GetQuestsNearDeadline();
            var gameChallenges = GetQuestsWithGameChallenge();

            return $"📊 Quest Summary:\n" +
                   $"⚔️ Active Quests: {activeQuests.Count}\n" +
                   $"✅ Completed Quests: {completedQuests.Count}\n" +
                   $"⚠️ Quests Near Deadline: {nearDeadline.Count}\n" +
                   $"🎮 Pending Game Challenges: {gameChallenges.Count}\n" +
                   $"🏆 Total Quests: {_quests.Count}";
        }

        public string GetQuestStatus(string id)
        {
            var quest = GetQuestById(id);
            if (quest == null) return "Quest not found";

            var status = quest.IsCompleted ? "✅ COMPLETED" : "⚔️ ACTIVE";
            var gameStatus = quest.RequiresGameCompletion 
                ? (quest.IsGameCompleted ? "🎮 Game Challenge: COMPLETED" : "🎮 Game Challenge: PENDING") 
                : "🎮 No Game Challenge";

            return $"[{quest.Id}] {quest.Title}\n" +
                   $"Status: {status}\n" +
                   $"{gameStatus}\n" +
                   $"Due: {quest.DueDate:yyyy-MM-dd} | Priority: {quest.Priority}";
        }

        public List<Quest> GetAllQuests()
        {
            return new List<Quest>(_quests);
        }

        public List<Quest> GetActiveQuests()
        {
            return _quests.Where(q => !q.IsCompleted).ToList();
        }

        public List<Quest> GetCompletedQuests()
        {
            return _quests.Where(q => q.IsCompleted).ToList();
        }

        public List<Quest> GetQuestsNearDeadline()
        {
            return _quests.Where(q => q.IsNearDeadline()).ToList();
        }

        public Quest GetQuestById(string id)
        {
            return _quests.FirstOrDefault(q => q.Id.ToString() == id);
        }

        public bool UpdateQuest(string id, string title, string description, DateTime dueDate, Priority priority)
        {
            var quest = GetQuestById(id);
            if (quest != null)
            {
                quest.Title = title;
                quest.Description = description;
                quest.DueDate = dueDate;
                quest.Priority = priority;
                return true;
            }
            return false;
        }
     
    }
}