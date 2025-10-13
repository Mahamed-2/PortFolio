// Interfaces/IGuildAdvisorAI.cs
using System;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface IGuildAdvisorAI
    {
        Task<string> GenerateQuestDescriptionAsync(string title);
        Task<Priority> SuggestPriorityAsync(string title, DateTime dueDate);
        Task<string> SummarizeQuestsAsync(IQuestManager questManager); // CHANGED: Use interface
    }
}