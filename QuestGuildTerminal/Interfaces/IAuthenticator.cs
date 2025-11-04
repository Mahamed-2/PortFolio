// Interfaces/IAuthenticator.cs
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface IAuthenticator
    {
        Task<bool> RegisterAsync(Hero hero);
        Task<Hero> LoginAsync(string username, string password);
        bool Verify2FA(string code);
        Task Send2FACodeAsync(Hero hero); 
        Task Resend2FACodeAsync(string username); 
            string Generate2FACode(); // Add this method
 // New methods for Option 2
        Hero GetPendingHero(string username);
        bool HasPending2FA(string username);
        
    }
}