// Interfaces/INotificationService.cs - Update the method signature
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message, string contactInfo);
        Task CheckDeadlineNotificationsAsync(IQuestManager questManager, string heroContact);
        Task Send2FACodeNotificationAsync(string contactInfo, string code, string heroName);
    }
}