// Interfaces/IQuestManager.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface IQuestManager
    {
        void AddQuest(Quest quest);
        List<Quest> GetAllQuests();
        List<Quest> GetActiveQuests();
        List<Quest> GetCompletedQuests();
        List<Quest> GetQuestsNearDeadline();
        Quest GetQuestById(string id);
        bool CompleteQuest(string id);
        bool UpdateQuest(string id, string title, string description, DateTime dueDate, Priority priority);
        string GetQuestSummary();
    }
}