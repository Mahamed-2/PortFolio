// Services/GuildAdvisorAI.cs (if it exists)
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class GuildAdvisorAI : IGuildAdvisorAI
    {
        private readonly Random _random = new Random();
        
        public async Task<string> GenerateQuestDescriptionAsync(string title)
        {
            Console.WriteLine("🎭 [SIMULATED AI] Generating quest description...");
            await Task.Delay(800); // Simulate processing
            
            var descriptions = new System.Collections.Generic.Dictionary<string, string>
            {
                { "dragon", $"Brave hero! The ancient dragon {title} threatens our kingdom. You must venture into the fiery mountains and face the beast in its lair!" },
                { "village", $"The peaceful village of {title} is under siege! Defend the innocent and restore hope to the townsfolk in this crucial mission!" },
                { "artifact", $"The legendary {title} has been lost for centuries. Journey through forgotten ruins and overcome mystical guardians to reclaim it!" }
            };

            var key = descriptions.Keys.FirstOrDefault(k => title.ToLower().Contains(k)) ?? "default";
            
            return key == "default" 
                ? $"The quest '{title}' awaits a true hero! This mission will test your skills, courage, and wisdom. The guild has full confidence in your abilities!"
                : descriptions[key];
        }

        public async Task<Priority> SuggestPriorityAsync(string title, DateTime dueDate)
        {
            Console.WriteLine("🎭 [SIMULATED AI] Suggesting priority...");
            await Task.Delay(300);
            
            return CalculatePriority(title, dueDate);
        }

        // CHANGED: Accept IQuestManager instead of QuestManager
        public async Task<string> SummarizeQuestsAsync(IQuestManager questManager)
        {
            Console.WriteLine("🎭 [SIMULATED AI] Generating quest summary...");
            await Task.Delay(500);
            
            return GetFallbackSummary(questManager);
        }

        private Priority CalculatePriority(string title, DateTime dueDate)
        {
            var daysUntilDue = (dueDate - DateTime.Now).TotalDays;
            
            if (daysUntilDue <= 1) return Priority.High;
            if (daysUntilDue <= 3) return Priority.Medium;
            
            var urgentKeywords = new[] { "urgent", "emergency", "immediate", "critical", "save", "rescue" };
            if (urgentKeywords.Any(keyword => title.ToLower().Contains(keyword)))
                return Priority.High;
                
            return Priority.Low;
        }

        private string GetFallbackSummary(IQuestManager questManager)
        {
            var activeQuests = questManager.GetActiveQuests();
            var completedQuests = questManager.GetCompletedQuests();
            var nearDeadline = questManager.GetQuestsNearDeadline();

            var summary = "🎯 [SIMULATED] Guild Advisor's Briefing:\n\n";
            
            if (nearDeadline.Any())
            {
                summary += $"⚡ URGENT: {nearDeadline.Count} quest(s) require immediate attention!\n";
            }

            summary += $"⚔️ Active Missions: {activeQuests.Count}\n";
            summary += $"✅ Completed Quests: {completedQuests.Count}\n";
            summary += "🏆 Continue your heroic journey!";

            return summary;
        }
    }
}