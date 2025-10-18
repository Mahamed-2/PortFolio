// Services/DatabaseAuthenticator.cs (Proper async version)
using System;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class DatabaseAuthenticator : IAuthenticator
    {
        private string _connectionString;

        public DatabaseAuthenticator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> RegisterAsync(Hero hero)
        {
            // Add actual async database operations
            await Task.Delay(100); // Simulate async work
            // Your actual registration logic here
            return true;
        }

        public async Task<Hero> LoginAsync(string username, string password)
        {
            // Add actual async database operations
            await Task.Delay(100); // Simulate async work
            // Your actual login logic here
            return null;
        }

        public async Task Send2FACodeAsync(Hero hero)
        {
            // Add actual async operations
            await Task.Delay(100); // Simulate async work
            Console.WriteLine($"📱 2FA code sent to {hero.Email}");
        }

        public bool Verify2FA(string code)
        {
            // This can stay synchronous
            return code == "123456"; // Example
        }

        public async Task Resend2FACodeAsync(string username)
        {
            // Add actual async operations
            await Task.Delay(100); // Simulate async work
            Console.WriteLine($"📱 2FA code resent to {username}");
        }
    }
}