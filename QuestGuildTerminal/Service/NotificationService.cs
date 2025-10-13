// Services/NotificationService.cs
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class NotificationService : INotificationService
    {
        public async Task SendNotificationAsync(string message, string contactInfo)
        {
            // Simulate sending notification
            Console.WriteLine($"\n📢 Guild Alert to {contactInfo}:");
            Console.WriteLine($"💬 {message}");
            await Task.Delay(100);
        }

        // FIXED: Updated to accept IQuestManager instead of QuestManager
        public async Task CheckDeadlineNotificationsAsync(IQuestManager questManager, string heroContact)
        {
            var nearDeadlineQuests = questManager.GetQuestsNearDeadline();
            
            foreach (var quest in nearDeadlineQuests)
            {
                var message = $"⚔️ Hero, your quest '{quest.Title}' must be completed by tomorrow!";
                await SendNotificationAsync(message, heroContact);
            }

            if (!nearDeadlineQuests.Any())
            {
                Console.WriteLine("\n✅ No urgent deadline notifications at this time.");
            }
        }

        public async Task Send2FACodeNotificationAsync(string contactInfo, string code, string heroName)
        {
            Console.WriteLine($"\n🔐 2FA Code sent to {contactInfo}: {code}");
            Console.WriteLine("💡 In production, this would be sent via real email/SMS");
            await Task.Delay(100);
        }
    }
}