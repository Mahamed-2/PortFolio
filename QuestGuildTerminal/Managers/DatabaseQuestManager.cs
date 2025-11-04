// Managers/DatabaseQuestManager.cs (updated)
using QuestGuildTerminal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuestGuildTerminal
{
    public class DatabaseQuestManager : IQuestManager
    {
        private readonly QuestGuildContext _context;
        private readonly GameManager _gameManager;

        // MODIFIED: Add GameManager dependency
        public DatabaseQuestManager(QuestGuildContext context, GameManager gameManager = null)
        {
            _context = context;
            _gameManager = gameManager;
        }

        public async void AddQuest(Quest quest)
        {
            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();
        }

        // NEW: Add quest with game challenge option
        public void AddQuest(Quest quest, bool includeGameChallenge)
        {
            if (includeGameChallenge && _gameManager != null)
            {
                quest.RequiresGameCompletion = true;
                quest.RequiredGame = "Tetris";
                quest.RequiredGameLevel = 3;
            }
            
            _context.Quests.Add(quest);
            _context.SaveChanges(); // Synchronous for interface compatibility
        }

        public List<Quest> GetAllQuests()
        {
            return _context.Quests.ToList();
        }

        public List<Quest> GetActiveQuests()
        {
            return _context.Quests
                .Where(q => !q.IsCompleted)
                .OrderBy(q => q.DueDate)
                .ToList();
        }

        public List<Quest> GetCompletedQuests()
        {
            return _context.Quests
                .Where(q => q.IsCompleted)
                .OrderByDescending(q => q.CompletedDate)
                .ToList();
        }

        public List<Quest> GetQuestsNearDeadline()
        {
            return _context.Quests
                .Where(q => !q.IsCompleted && q.DueDate <= DateTime.Now.AddHours(24))
                .OrderBy(q => q.DueDate)
                .ToList();
        }

        public Quest GetQuestById(string id)
        {
            if (int.TryParse(id, out int questId))
            {
                return _context.Quests.Find(questId);
            }
            return null;
        }

        public bool CompleteQuest(string id)
        {
            if (int.TryParse(id, out int questId))
            {
                var quest = _context.Quests.Find(questId);
                if (quest != null && !quest.IsCompleted)
                {
                    // NEW: Check if game challenge is required but not completed
                    if (quest.RequiresGameCompletion && !quest.IsGameCompleted)
                    {
                        return false; // Cannot complete without finishing game challenge
                    }

                    quest.MarkComplete();
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool UpdateQuest(string id, string title, string description, DateTime dueDate, Priority priority)
        {
            if (int.TryParse(id, out int questId))
            {
                var quest = _context.Quests.Find(questId);
                if (quest != null)
                {
                    quest.Title = title;
                    quest.Description = description;
                    quest.DueDate = dueDate;
                    quest.Priority = priority;
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public string GetQuestSummary()
        {
            var activeQuests = GetActiveQuests();
            var completedQuests = GetCompletedQuests();
            var nearDeadline = GetQuestsNearDeadline();
            var gameChallenges = GetQuestsWithGameChallenge(); // NEW: Include game challenges

            return $"📊 Quest Summary:\n" +
                   $"⚔️ Active Quests: {activeQuests.Count}\n" +
                   $"✅ Completed Quests: {completedQuests.Count}\n" +
                   $"⚠️ Quests Near Deadline: {nearDeadline.Count}\n" +
                   $"🎮 Pending Game Challenges: {gameChallenges.Count}\n" + // NEW: Game challenges count
                   $"🏆 Total Quests: {activeQuests.Count + completedQuests.Count}";
        }

        // NEW: Game challenge completion method
        public async Task<bool> AttemptQuestCompletionAsync(string id)
        {
            var quest = GetQuestById(id);
            if (quest == null || quest.IsCompleted)
                return false;

            // If no game requirement, complete immediately
            if (!quest.RequiresGameCompletion || _gameManager == null)
            {
                quest.MarkComplete();
                _context.SaveChanges();
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
                        quest.MarkComplete(); // Auto-complete after game success
                        _context.SaveChanges();
                        
                        Console.WriteLine($"\n🎉 Challenge Completed! Reached Level {result.FinalLevel}");
                        Console.WriteLine($"🏆 Score: {result.Score}");
                        Console.WriteLine("✅ Quest completed successfully!");
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
                    _context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        // NEW: Get quests that need game completion
        public List<Quest> GetQuestsWithGameChallenge()
        {
            return _context.Quests
                .Where(q => q.RequiresGameCompletion && !q.IsGameCompleted && !q.IsCompleted)
                .ToList();
        }

        // NEW: Check if quest has pending game challenge
        public bool HasPendingGameChallenge(string id)
        {
            var quest = GetQuestById(id);
            return quest?.RequiresGameCompletion == true && !quest.IsGameCompleted;
        }

        // NEW: Get detailed quest status
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

        // Keep the async methods for future use
        public async Task AddQuestAsync(Quest quest)
        {
            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Quest>> GetAllQuestsAsync(int heroId)
        {
            return await _context.Quests
                .Where(q => q.HeroId == heroId)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }
    }
}