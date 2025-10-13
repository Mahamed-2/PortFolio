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

        public DatabaseQuestManager(QuestGuildContext context)
        {
            _context = context;
        }

        public async void AddQuest(Quest quest)
        {
            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();
        }

        public List<Quest> GetAllQuests()
        {
            // For simplicity, we'll make this synchronous for now
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

            return $"📊 Quest Summary:\n" +
                   $"⚔️ Active Quests: {activeQuests.Count}\n" +
                   $"✅ Completed Quests: {completedQuests.Count}\n" +
                   $"⚠️ Quests Near Deadline: {nearDeadline.Count}\n" +
                   $"🏆 Total Quests: {activeQuests.Count + completedQuests.Count}";
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

        // ... other async methods remain for future database usage
    }
}