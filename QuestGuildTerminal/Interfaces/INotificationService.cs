// Interfaces/INotificationService.cs - Update the method signature
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message, string contactInfo);
        Task CheckDeadlineNotificationsAsync(IQuestManager questManager, string heroContact); // CHANGED: Use interface
        Task Send2FACodeNotificationAsync(string contactInfo, string code, string heroName);
    }
}