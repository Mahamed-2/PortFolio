

namespace QuestGuildTerminal
{
    public class EnhancedGuildAdvisorAI : IGuildAdvisorAI
    {
        private readonly Random _random = new Random();
        
        // Enhanced simulated responses that feel more realistic
        private readonly string[] _questTemplates = {
            "Brave adventurer! The {0} awaits your courage. Journey through treacherous lands, face formidable foes, and return with glory! The kingdom's hope rests upon your shoulders!",
            "Hero of renown! The {0} calls for your expertise. Navigate ancient ruins, solve mystical puzzles, and claim the legendary treasure that lies within!",
            "Valiant warrior! The {0} demands your strength. Battle dark forces, protect the innocent, and restore peace to the troubled realm! Your legend begins now!",
            "Wise traveler! The {0} requires your wisdom. Uncover forgotten secrets, outsmart cunning adversaries, and bring enlightenment to all!",
            "Noble champion! The {0} needs your valor. Confront the shadowy threat, overcome impossible odds, and write your name in the annals of history!"
        };
        
        private readonly string[] _summaryTemplates = {
            "Heroic one, your current endeavors shine with promise! {0} active quests demand your attention, with {1} requiring urgent action. {2} glorious victories already adorn your legacy!",
            "Valiant warrior, the guild observes your progress with pride! {0} missions await your prowess, {1} of which call for immediate valor. {2} triumphs already echo through the halls!",
            "Brave soul, your quest log brims with potential glory! {0} adventures call your name, {1} with pressing deadlines. {2} conquests already testify to your might!",
            "Noble hero, your journey inspires all who witness it! {0} challenges stand before you, {1} demanding swift action. {2} achievements already mark your legendary path!"
        };

        public async Task<string> GenerateQuestDescriptionAsync(string title)
        {
            Console.WriteLine("🎭 Guild Advisor is crafting your quest description...");
            await Task.Delay(800 + _random.Next(400)); // Realistic delay
            
            var template = _questTemplates[_random.Next(_questTemplates.Length)];
            return string.Format(template, title);
        }

        public async Task<Priority> SuggestPriorityAsync(string title, DateTime dueDate)
        {
            Console.WriteLine("🎭 Guild Advisor is analyzing priority...");
            await Task.Delay(300 + _random.Next(200));
            
            var daysUntilDue = (dueDate - DateTime.Now).TotalDays;
            var urgency = title.ToLower();
            
            // Smart simulated priority logic
            if (daysUntilDue <= 1 || urgency.Contains("urgent") || urgency.Contains("emergency"))
                return Priority.High;
            if (daysUntilDue <= 3 || urgency.Contains("important") || urgency.Contains("critical"))
                return Priority.Medium;
            return Priority.Low;
        }

        // CHANGED: Accept IQuestManager instead of QuestManager
        public async Task<string> SummarizeQuestsAsync(IQuestManager questManager)
        {
            Console.WriteLine("🎭 Guild Advisor is preparing your briefing...");
            await Task.Delay(500 + _random.Next(300));
            
            var activeQuests = questManager.GetActiveQuests();
            var completedQuests = questManager.GetCompletedQuests();
            var nearDeadline = questManager.GetQuestsNearDeadline();

            if (!activeQuests.Any())
            {
                return "Wise hero, your quest log stands empty, a blank canvas awaiting your next great masterpiece! The guild eagerly anticipates your next legendary undertaking!";
            }

            var template = _summaryTemplates[_random.Next(_summaryTemplates.Length)];
            var summary = string.Format(template, activeQuests.Count, nearDeadline.Count, completedQuests.Count);
            
            // Add specific mentions for near-deadline quests
            if (nearDeadline.Any())
            {
                summary += $"\n\n⚡ URGENT: {string.Join(", ", nearDeadline.Take(3).Select(q => q.Title))} " +
                          $"{(nearDeadline.Count > 3 ? "and more" : "")} require your immediate valor!";
            }
            
            // Add motivational ending
            var motivations = new[]
            {
                "\n\n🏹 The fates weave patterns of glory around your path!",
                "\n\n⚔️ Your legend grows with every challenge faced!",
                "\n\n🛡️ Courage, hero! The realm watches with bated breath!",
                "\n\n💫 Destiny's threads align in your favor, brave one!"
            };
            
            summary += motivations[_random.Next(motivations.Length)];
            
            return summary;
        }
    }
}