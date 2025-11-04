// Services/Authenticator.cs
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class Authenticator : IAuthenticator
    {
        private Dictionary<string, string> _pending2FACodes;
        private Dictionary<string, Hero> _pendingHeroes; // Store heroes temporarily
        private string _connectionString;
        private readonly INotificationService _notificationService;

        public Authenticator(INotificationService notificationService = null)
        {
            _pending2FACodes = new Dictionary<string, string>();
            _pendingHeroes = new Dictionary<string, Hero>(); // Initialize
            _connectionString = DatabaseConfig.ConnectionString;
            _notificationService = notificationService ?? new NotificationService();
        }

        public async Task<bool> RegisterAsync(Hero hero)
        {
            // Keep your existing registration code exactly as is
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // First check if username already exists
                    string checkQuery = "SELECT COUNT(*) FROM Heroes WHERE Username = @Username";
                    using (var checkCommand = new SqliteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", hero.Username);
                        var existingCount = Convert.ToInt64(await checkCommand.ExecuteScalarAsync());

                        if (existingCount > 0)
                        {
                            Console.WriteLine($"❌ Username '{hero.Username}' already exists in database");
                            return false;
                        }
                    }

                    // Insert new hero
                    string insertQuery = @"
                INSERT INTO Heroes (Username, Password, Email, Phone, Level, Experience, Class) 
                VALUES (@Username, @Password, @Email, @Phone, @Level, @Experience, @Class)";

                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", hero.Username);
                        command.Parameters.AddWithValue("@Password", hero.Password);
                        command.Parameters.AddWithValue("@Email", hero.Email);
                        command.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(hero.Phone) ? DBNull.Value : (object)hero.Phone);
                        command.Parameters.AddWithValue("@Level", hero.Level);
                        command.Parameters.AddWithValue("@Experience", hero.Experience);
                        command.Parameters.AddWithValue("@Class", hero.Class);

                        int result = await command.ExecuteNonQueryAsync();
                        
                        // Send welcome notification
                        if (result > 0 && _notificationService != null)
                        {
                            var contactInfo = hero.Email ?? hero.Phone;
                            if (!string.IsNullOrEmpty(contactInfo))
                            {
                                // Use the two-argument overload (title, message) of SendNotificationAsync
                                await _notificationService.SendNotificationAsync(
                                    "🎉 Welcome to the Quest Guild!",
                                    $"Hero {hero.Username}, your registration is complete! Begin your adventures!"
                                );
                            }
                        }
                        
                        Console.WriteLine($"✅ Hero registered successfully. Rows affected: {result}");
                        return result > 0;
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"❌ SQLite error during registration: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Registration failed: {ex.Message}");
                return false;
            }
        }

        public async Task<Hero> LoginAsync(string username, string password)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    string query = "SELECT * FROM Heroes WHERE Username = @Username AND Password = @Password";
                    
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var hero = new Hero
                                {
                                    Id = GetSafeInt(reader, "Id"),
                                    Username = GetSafeString(reader, "Username"),
                                    Password = GetSafeString(reader, "Password"),
                                    Email = GetSafeString(reader, "Email"),
                                    Phone = GetSafeString(reader, "Phone"),
                                    Level = GetSafeInt(reader, "Level", 1),
                                    Experience = GetSafeInt(reader, "Experience", 0),
                                    Class = GetSafeString(reader, "Class", "Adventurer")
                                };
                                
                                // Store the hero temporarily and send 2FA
                                _pendingHeroes[username] = hero;
                                await Send2FACodeAsync(hero);
                                
                                return null; // Return null to indicate 2FA is pending
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login failed: {ex.Message}");
                return null;
            }
            return null;
        }

        public async Task Send2FACodeAsync(Hero hero)
        {
            var code = Generate2FACode();
            _pending2FACodes[hero.Username] = code;

            // Use NotificationService
            var contactInfo = hero.Email ?? hero.Phone;
            if (!string.IsNullOrEmpty(contactInfo) && _notificationService != null)
            {
                await _notificationService.Send2FACodeNotificationAsync(contactInfo, code, hero.Username);
            }
            else
            {
                // Fallback
                Console.WriteLine($"\n📱 2FA Code for {hero.Username}: {code}");
            }
        }

        public bool Verify2FA(string code)
        {
            // Find which user has this pending 2FA
            foreach (var kvp in _pending2FACodes)
            {
                if (kvp.Value == code)
                {
                    _pending2FACodes.Remove(kvp.Key);
                    return true;
                }
            }
            return false;
        }

        public Hero GetPendingHero(string username)
        {
            if (_pendingHeroes.TryGetValue(username, out var hero))
            {
                _pendingHeroes.Remove(username); // Clear after retrieval
                return hero;
            }
            return null;
        }

        public bool HasPending2FA(string username)
        {
            return _pending2FACodes.ContainsKey(username);
        }

        // Helper methods
        public string Generate2FACode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GetSafeString(SqliteDataReader reader, string columnName, string defaultValue = "")
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
            }
            catch
            {
                return defaultValue;
            }
        }

        private int GetSafeInt(SqliteDataReader reader, string columnName, int defaultValue = 0)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
            }
            catch
            {
                return defaultValue;
            }
        }

        public async Task Resend2FACodeAsync(string username)
        {
            if (_pendingHeroes.TryGetValue(username, out var hero))
            {
                await Send2FACodeAsync(hero);
            }
        }
    }
}