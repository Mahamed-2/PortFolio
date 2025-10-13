// Services/DatabaseAuthenticator.cs
using QuestGuildTerminal.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class DatabaseAuthenticator : IAuthenticator
    {
        private readonly QuestGuildContext _context;
        private readonly INotificationService _notificationService;
        private Dictionary<string, string> _pending2FACodes;
        private Dictionary<string, DateTime> _codeExpiry;
        private string _currentHeroUsername;

        public DatabaseAuthenticator(QuestGuildContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
            _pending2FACodes = new Dictionary<string, string>();
            _codeExpiry = new Dictionary<string, DateTime>();
        }

        public async Task<bool> RegisterAsync(Hero hero)
        {
            // Check if username or email already exists
            var existingHero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.Username == hero.Username || h.Email == hero.Email);

            if (existingHero != null)
            {
                return false;
            }

            _context.Heroes.Add(hero);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Hero> LoginAsync(string username, string password)
        {
            var hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.Username == username);

            if (hero != null && hero.VerifyPassword(password))
            {
                _currentHeroUsername = username;
                
                // Update last login
                hero.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();
                
                await Send2FACodeAsync(hero);
                return hero;
            }
            return null;
        }

        public async Task Send2FACodeAsync(Hero hero)
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            _pending2FACodes[hero.Username] = code;
            _codeExpiry[hero.Username] = DateTime.Now.AddMinutes(10);

            Console.WriteLine($"\n🔐 Sending 2FA code to {hero.Email}...");
            
            await _notificationService.Send2FACodeNotificationAsync(hero.Email, code, hero.Username);
        }

        public bool Verify2FA(string code)
        {
            if (_pending2FACodes.TryGetValue(_currentHeroUsername, out var storedCode) &&
                _codeExpiry.TryGetValue(_currentHeroUsername, out var expiry))
            {
                if (storedCode == code && DateTime.Now <= expiry)
                {
                    _pending2FACodes.Remove(_currentHeroUsername);
                    _codeExpiry.Remove(_currentHeroUsername);
                    return true;
                }
            }
            return false;
        }

        public async Task Resend2FACodeAsync(string username)
        {
            var hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.Username == username);
                
            if (hero != null)
            {
                await Send2FACodeAsync(hero);
            }
        }

        public async Task<Hero> GetHeroByUsernameAsync(string username)
        {
            return await _context.Heroes
                .Include(h => h.Quests)
                .Include(h => h.Achievements)
                .FirstOrDefaultAsync(h => h.Username == username);
        }
    }
}