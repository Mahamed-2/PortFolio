// Interfaces/IQuestManager.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface IQuestManager
    {
        // Existing methods
        void AddQuest(Quest quest);
        void AddQuest(Quest quest, bool includeGameChallenge); // NEW: Overload for game challenges
        List<Quest> GetAllQuests();
        List<Quest> GetActiveQuests();
        List<Quest> GetCompletedQuests();
        List<Quest> GetQuestsNearDeadline();
        Quest GetQuestById(string id);
        bool CompleteQuest(string id);
        bool UpdateQuest(string id, string title, string description, DateTime dueDate, Priority priority);
        string GetQuestSummary();

        // NEW: Game-related methods
        Task<bool> AttemptQuestCompletionAsync(string id);
        List<Quest> GetQuestsWithGameChallenge();
        bool HasPendingGameChallenge(string id);
        string GetQuestStatus(string id);
    }
}