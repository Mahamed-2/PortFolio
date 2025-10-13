// Interfaces/IAuthenticator.cs
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface IAuthenticator
    {
        Task<bool> RegisterAsync(Hero hero);
        Task<Hero> LoginAsync(string username, string password);
        bool Verify2FA(string code);
        Task Send2FACodeAsync(Hero hero); // CHANGED: From void to Task
        Task Resend2FACodeAsync(string username); // ADDED: New method
    }
}