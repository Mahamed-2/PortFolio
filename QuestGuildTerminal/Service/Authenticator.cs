// Services/Authenticator.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class Authenticator : IAuthenticator
    {
        private Dictionary<string, Hero> _heroes;
        private Dictionary<string, string> _pending2FACodes;
        private string _currentHeroUsername;

        public Authenticator()
        {
            _heroes = new Dictionary<string, Hero>();
            _pending2FACodes = new Dictionary<string, string>();
        }

        public async Task<bool> RegisterAsync(Hero hero)
        {
            if (_heroes.ContainsKey(hero.Username))
            {
                return false;
            }

            _heroes[hero.Username] = hero;
            await Task.Delay(500); // Simulate async operation
            return true;
        }

        public async Task<Hero> LoginAsync(string username, string password)
        {
            await Task.Delay(500); // Simulate async operation
            
            if (_heroes.TryGetValue(username, out var hero) && hero.Password == password)
            {
                _currentHeroUsername = username;
                await Send2FACodeAsync(hero); // CHANGED: Now awaitable
                return hero;
            }
            return null;
        }

        // CHANGED: From void to async Task to match interface
        public async Task Send2FACodeAsync(Hero hero)
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            _pending2FACodes[hero.Username] = code;

            // In a real app, this would send SMS/Email
            Console.WriteLine($"\n📱 2FA Code sent to {hero.Email}: {code}");
            Console.WriteLine("💡 In production, this would be sent via SMS/Email");
            
            await Task.Delay(100); // Simulate sending
        }

        public bool Verify2FA(string code)
        {
            if (_pending2FACodes.TryGetValue(_currentHeroUsername, out var storedCode))
            {
                if (storedCode == code)
                {
                    _pending2FACodes.Remove(_currentHeroUsername);
                    return true;
                }
            }
            return false;
        }

        // ADD THIS MISSING METHOD to implement the interface
        public async Task Resend2FACodeAsync(string username)
        {
            if (_heroes.TryGetValue(username, out var hero))
            {
                await Send2FACodeAsync(hero);
            }
        }
    }
}