// Services/Authenticator.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class Authenticator : IAuthenticator
    {
        private Dictionary<string, string> _pending2FACodes;
        private string _currentHeroUsername;
        private string _connectionString;

        public Authenticator()
        {
            _pending2FACodes = new Dictionary<string, string>();
            _connectionString = DatabaseConfig.ConnectionString;
        }

      // Services/Authenticator.cs 
public async Task<bool> RegisterAsync(Hero hero)
{
    try
    {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // First check if username already exists
                    string checkQuery = "SELECT COUNT(*) FROM Heroes WHERE Username = @Username";
                    using (var checkCommand = new Microsoft.Data.Sqlite.SqliteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", hero.Username);
                        var existingCount = Convert.ToInt64(await checkCommand.ExecuteScalarAsync());

                        if (existingCount > 0)
                        {
                            Console.WriteLine($"❌ Username '{hero.Username}' already exists in database");
                            return false;
                        }
                    }

                    // Insert new hero with proper null handling for Phone
                    string insertQuery = @"
                INSERT INTO Heroes (Username, Password, Email, Phone, Level, Experience, Class) 
                VALUES (@Username, @Password, @Email, @Phone, @Level, @Experience, @Class)";

                    using (var command = new Microsoft.Data.Sqlite.SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", hero.Username);
                        command.Parameters.AddWithValue("@Password", hero.Password);
                        command.Parameters.AddWithValue("@Email", hero.Email);

                        // Handle Phone being null
                        if (string.IsNullOrEmpty(hero.Phone))
                        {
                            command.Parameters.AddWithValue("@Phone", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Phone", hero.Phone);
                        }

                        command.Parameters.AddWithValue("@Level", hero.Level);
                        command.Parameters.AddWithValue("@Experience", hero.Experience);
                        command.Parameters.AddWithValue("@Class", hero.Class);

                        int result = await command.ExecuteNonQueryAsync();
                        Console.WriteLine($"✅ Hero registered successfully. Rows affected: {result}");
                        return result > 0;
                    }

                }
    }
    catch (Microsoft.Data.Sqlite.SqliteException ex)
    {
        Console.WriteLine($"❌ SQLite error during registration: {ex.Message} (Error code: {ex.SqliteErrorCode})");
        
        if (ex.SqliteErrorCode == 1) // SQLITE_ERROR
        {
            Console.WriteLine("💡 This usually means the Heroes table doesn't exist.");
            Console.WriteLine("💡 Please ensure DatabaseInitializer.InitializeDatabaseAsync() runs successfully.");
        }
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Registration failed: {ex.Message}");
        return false;
    }
}

// Services/Authenticator.cs - Update LoginAsync method
public async Task<Hero> LoginAsync(string username, string password)
{
    try
    {
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            string query = "SELECT * FROM Heroes WHERE Username = @Username AND Password = @Password";
            
            using (var command = new Microsoft.Data.Sqlite.SqliteCommand(query, connection))
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
                            Level = GetSafeInt(reader, "Level", 1), // Default to 1 if column missing
                            Experience = GetSafeInt(reader, "Experience", 0), // Default to 0
                            Class = GetSafeString(reader, "Class", "Adventurer") // Default value
                        };
                        
                        _currentHeroUsername = username;
                        await Send2FACodeAsync(hero);
                        return hero;
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

// Add these helper methods to Authenticator class
private string GetSafeString(Microsoft.Data.Sqlite.SqliteDataReader reader, string columnName, string defaultValue = "")
{
    try
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
    }
    catch (Exception ex) when (ex is IndexOutOfRangeException || ex is ArgumentException)
    {
        Console.WriteLine($"⚠️ Column '{columnName}' not found, using default: {defaultValue}");
        return defaultValue;
    }
}

private int GetSafeInt(Microsoft.Data.Sqlite.SqliteDataReader reader, string columnName, int defaultValue = 0)
{
    try
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
    }
    catch (Exception ex) when (ex is IndexOutOfRangeException || ex is ArgumentException)
    {
        Console.WriteLine($"⚠️ Column '{columnName}' not found, using default: {defaultValue}");
        return defaultValue;
    }
}

        // ... rest of your existing methods (Send2FACodeAsync, Verify2FA, Resend2FACodeAsync) remain the same
        public async Task Send2FACodeAsync(Hero hero)
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            _pending2FACodes[hero.Username] = code;

            Console.WriteLine($"\n📱 2FA Code sent to {hero.Email}: {code}");
            Console.WriteLine("💡 In production, this would be sent via SMS/Email");
            
            await Task.Delay(100);
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

        public async Task Resend2FACodeAsync(string username)
        {
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Heroes WHERE Username = @Username";
                
                using (var command = new Microsoft.Data.Sqlite.SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var hero = new Hero
                            {
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString(reader.GetOrdinal("Phone"))
                            };
                            await Send2FACodeAsync(hero);
                        }
                    }
                }
            }
        }
    }
}