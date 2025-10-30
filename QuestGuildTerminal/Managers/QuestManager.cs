// Managers/QuestManager.cs - Make sure it explicitly implements IQuestManager
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestGuildTerminal
{
    public class QuestManager : IQuestManager 
    {
        private List<Quest> _quests;

        public QuestManager()
        {
            _quests = new List<Quest>();
        }

      
        public void AddQuest(Quest quest)
        {
            _quests.Add(quest);
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

        public bool CompleteQuest(string id)
        {
            var quest = GetQuestById(id);
            if (quest != null && !quest.IsCompleted)
            {
                quest.IsCompleted = true;
                return true;
            }
            return false;
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

        public string GetQuestSummary()
        {
            var activeQuests = GetActiveQuests();
            var completedQuests = GetCompletedQuests();
            var nearDeadline = GetQuestsNearDeadline();

            return $"📊 Quest Summary:\n" +
                   $"⚔️ Active Quests: {activeQuests.Count}\n" +
                   $"✅ Completed Quests: {completedQuests.Count}\n" +
                   $"⚠️ Quests Near Deadline: {nearDeadline.Count}\n" +
                   $"🏆 Total Quests: {_quests.Count}";
        }
    }
}