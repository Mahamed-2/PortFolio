// Program.cs
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuestGuildTerminal.Data; 
using QuestGuildTerminal.Services;


namespace QuestGuildTerminal
{
    class Program
    {
        // In Program.cs - update Main method// Program.cs - Update your Main method
        static async Task Main(string[] args)
        {
            Console.WriteLine("🏰 Welcome to the Quest Guild Terminal! 🏰");

            // Force complete rebuild
            if (args.Length > 0 && args[0] == "--rebuild")
            {
                await CompleteDatabaseRebuild();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                await DatabaseInitializer.InitializeDatabaseAsync();
                 await DatabaseInitializer.DebugDatabaseSchema(); 

                var app = new QuestGuildApp();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Failed: {ex.Message}");
                Console.WriteLine("💡 Try: dotnet run -- --rebuild");
                Console.ReadKey();
            }
        }


        // ADD DATABASE INITIALIZER DIRECTLY TO PROGRAM.CS
        public static async Task InitializeDatabaseAsync()
        {
            string dbPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");
            Console.WriteLine($"🔍 Database path: {dbPath}");

            try
            {
                bool isNewDatabase = !System.IO.File.Exists(dbPath);
                
                if (isNewDatabase)
                {
                    Console.WriteLine("🆕 Creating new database...");
                    System.IO.File.WriteAllBytes(dbPath, new byte[0]);
                    await Task.Delay(500);
                }
                else
                {
                    Console.WriteLine("📁 Using existing database...");
                }

                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}"))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Database connection opened");

                    if (isNewDatabase)
                    {
                        await CreateHeroesTable(connection);
                        await AddTestData(connection);
                        Console.WriteLine("🎉 New database created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("🔄 Checking database schema...");
                        await UpdateDatabaseSchema(connection);
                        Console.WriteLine("✅ Database schema updated!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database initialization FAILED: {ex.Message}");
                throw;
            }
        }

        private static async Task UpdateDatabaseSchema(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            try
            {
                bool phoneColumnExists = await CheckColumnExists(connection, "Heroes", "Phone");
                
                if (!phoneColumnExists)
                {
                    Console.WriteLine("🔧 Adding Phone column to Heroes table...");
                    string addPhoneColumn = "ALTER TABLE Heroes ADD COLUMN Phone TEXT";
                    using var command = new Microsoft.Data.Sqlite.SqliteCommand(addPhoneColumn, connection);
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine("✅ Added Phone column to Heroes table");
                }
                else
                {
                    Console.WriteLine("✅ Phone column already exists in Heroes table");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Schema update failed: {ex.Message}");
                throw;
            }
        }

        private static async Task<bool> CheckColumnExists(Microsoft.Data.Sqlite.SqliteConnection connection, string tableName, string columnName)
        {
            try
            {
                string sql = $@"
                    SELECT COUNT(*) FROM pragma_table_info('{tableName}') 
                    WHERE name = '{columnName}'";
                
                using var command = new Microsoft.Data.Sqlite.SqliteCommand(sql, connection);
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not check if column {columnName} exists: {ex.Message}");
                return false;
            }
        }

        private static async Task CreateHeroesTable(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            try
            {
                string sql = @"
                    CREATE TABLE Heroes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        Phone TEXT,
                        Level INTEGER DEFAULT 1,
                        Experience INTEGER DEFAULT 0,
                        Class TEXT DEFAULT 'Adventurer',
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                
                using var command = new Microsoft.Data.Sqlite.SqliteCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("✅ Heroes table created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to create Heroes table: {ex.Message}");
                throw;
            }
        }

        private static async Task CreateQuestsTable(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            try
            {
                string sql = @"
                    CREATE TABLE Quests (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        DueDate DATETIME NOT NULL,
                        Priority INTEGER NOT NULL,
                        IsCompleted INTEGER DEFAULT 0,
                        HeroId INTEGER,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                
                using var command = new Microsoft.Data.Sqlite.SqliteCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("✅ Quests table created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to create Quests table: {ex.Message}");
                throw;
            }
        }

        private static async Task AddTestData(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            try
            {
                string checkSql = "SELECT COUNT(*) FROM Heroes WHERE Username = 'testhero'";
                using var checkCommand = new Microsoft.Data.Sqlite.SqliteCommand(checkSql, connection);
                var existingCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                
                if (existingCount == 0)
                {
                    string insertSql = @"
                        INSERT INTO Heroes (Username, Password, Email, Phone) 
                        VALUES ('testhero', 'test123', 'test@example.com', '+1234567890')";
                    
                    using var command = new Microsoft.Data.Sqlite.SqliteCommand(insertSql, connection);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"✅ Test hero added. Rows affected: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine("ℹ️ Test hero already exists in database");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not add test data: {ex.Message}");
            }
        }


// Add this method to Program.cs
public static async Task EmergencyDatabaseReset()
{
    try
    {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
            Console.WriteLine("🗑️ Old database file deleted");
            await Task.Delay(1000); // Wait a moment
        }
        
        Console.WriteLine("🔄 Creating new database...");
        await DatabaseInitializer.InitializeDatabaseAsync();
        Console.WriteLine("✅ Database reset completed successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Emergency reset failed: {ex.Message}");
    }
}

// Add this method to test database connection
public static async Task TestDatabaseConnection()
{
    try
    {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");
        Console.WriteLine($"📁 Checking database file: {dbPath}");
        Console.WriteLine($"📁 File exists: {File.Exists(dbPath)}");
        
        if (!File.Exists(dbPath))
        {
            throw new FileNotFoundException("Database file not found!");
        }

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
        await connection.OpenAsync();
        Console.WriteLine("✅ Database connection successful");

        // Test Heroes table
        using var command = new Microsoft.Data.Sqlite.SqliteCommand("SELECT COUNT(*) FROM Heroes", connection);
        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        Console.WriteLine($"✅ Heroes table test: {count} heroes found");

        // Test Quests table
        using var command2 = new Microsoft.Data.Sqlite.SqliteCommand("SELECT COUNT(*) FROM Quests", connection);
        var questCount = Convert.ToInt32(await command2.ExecuteScalarAsync());
        Console.WriteLine($"✅ Quests table test: {questCount} quests found");

        Console.WriteLine("🎉 Database connection test PASSED!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database connection test FAILED: {ex.Message}");
        throw;
    }
}

        static async Task TestNotificationServices()
        {
            Console.WriteLine("🧪 Testing notification services...");
            
            var notificationConfig = AppConfig.GetNotificationConfig();
            var notificationService = new EnhancedNotificationService(notificationConfig);
            
            // Test email - use the correct method name
            if (notificationConfig.IsEmailEnabled)
            {
                Console.WriteLine("📧 Testing email service...");
                await notificationService.Send2FACodeNotificationAsync( // FIXED: Correct method name
                    "test@example.com", 
                    "123456", 
                    "TestHero"
                );
            }
            
            // Test SMS
            if (notificationConfig.IsSmsEnabled)
            {
                Console.WriteLine("📱 Testing SMS service...");
                await notificationService.SendNotificationAsync(
                    "Test message from Quest Guild", 
                    "+1234567890"  // Use your phone number for testing
                );
            }
            
            if (!notificationConfig.IsEmailEnabled && !notificationConfig.IsSmsEnabled)
            {
                Console.WriteLine("ℹ️ Real notifications not configured - using simulated services");
            }
        }

        // ADD THIS METHOD: Complete API key tester
        public static async Task TestApiKey(string apiKey)
        {
            try
            {
                Console.WriteLine("🧪 Testing Gemini API Key...");
                Console.WriteLine($"🔑 API Key: {apiKey?.Substring(0, Math.Min(10, apiKey.Length))}...");
                
                using var httpClient = new HttpClient();
                
                // Try multiple endpoints
                var endpoints = new[]
                {
                    "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent",
                    "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro:generateContent", 
                    "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent",
                    "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent"
                };
                
                bool anyWorking = false;
                
                foreach (var endpoint in endpoints)
                {
                    try
                    {
                        var url = $"{endpoint}?key={apiKey}";
                        var requestBody = @"{
                            ""contents"": [{
                                ""parts"": [{
                                    ""text"": ""Say 'Hello World' in a heroic way""
                                }]
                            }]
                        }";
                        
                        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(url, content);
                        
                        Console.WriteLine($"🔍 {endpoint.Split('/').Last()} → {response.StatusCode}");
                        
                        if (response.IsSuccessStatusCode)
                        {
                            anyWorking = true;
                            var responseText = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"✅ SUCCESS with {endpoint.Split('/').Last()}!");
                            
                            // Try to extract the response text
                            try
                            {
                                var startIndex = responseText.IndexOf("\"text\": \"") + 9;
                                if (startIndex >= 9)
                                {
                                    var endIndex = responseText.IndexOf("\"", startIndex);
                                    if (endIndex > startIndex)
                                    {
                                        var aiResponse = responseText.Substring(startIndex, endIndex - startIndex);
                                        Console.WriteLine($"🤖 AI Response: {aiResponse}");
                                    }
                                }
                            }
                            catch (Exception parseEx)
                            {
                                Console.WriteLine($"⚠️ Could not parse response: {parseEx.Message}");
                            }
                            break; // Stop at first working endpoint
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"   Error: {errorContent.Substring(0, Math.Min(100, errorContent.Length))}...");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ {endpoint.Split('/').Last()} → Exception: {ex.Message}");
                    }
                    
                    await Task.Delay(500); // Small delay between attempts
                }
                
                if (!anyWorking)
                {
                    Console.WriteLine("\n❌ No working API endpoints found.");
                    Console.WriteLine("🔧 Using enhanced simulated AI instead.");
                    Console.WriteLine("💡 To enable real AI:");
                    Console.WriteLine("   1. Go to Google Cloud Console");
                    Console.WriteLine("   2. Enable 'Generative Language API'");
                    Console.WriteLine("   3. Ensure billing is set up");
                    Console.WriteLine("   4. Check API key restrictions");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 API Test Failed: {ex.Message}");
            }
        }

        // ADD THIS METHOD: List available models
        public static async Task ListAvailableModels(string apiKey)
        {
            try
            {
                Console.WriteLine("\n🔍 Checking available models...");

                using var httpClient = new HttpClient();
                var listUrl = $"https://generativelanguage.googleapis.com/v1/models?key={apiKey}";
                var response = await httpClient.GetAsync(listUrl);

                Console.WriteLine($"📡 Model List Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("✅ Available Models Response:");
                    Console.WriteLine(responseText.Length > 500 ? responseText.Substring(0, 500) + "..." : responseText);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Cannot list models: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Model List Failed: {ex.Message}");
            }
        }
        public static async Task DebugDatabaseCreation()
        {
            Console.WriteLine("🔍 DEBUG: Database Creation Process");
            Console.WriteLine("====================================");

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");
            Console.WriteLine($"📁 Database path: {dbPath}");

            // Delete any existing database
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
                Console.WriteLine("🗑️ Deleted existing database file");
                await Task.Delay(1000);
            }

            try
            {
                // Create the database file manually
                File.WriteAllBytes(dbPath, new byte[0]);
                Console.WriteLine("✅ Created new database file");

                using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
                await connection.OpenAsync();
                Console.WriteLine("✅ Database connection opened");

                // Create Heroes table with simple SQL
                Console.WriteLine("🏗️ Creating Heroes table...");
                var createTableCommand = new Microsoft.Data.Sqlite.SqliteCommand(@"
            CREATE TABLE Heroes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL,
                Email TEXT NOT NULL
            )", connection);

                await createTableCommand.ExecuteNonQueryAsync();
                Console.WriteLine("✅ Heroes table created");

                // Add test hero
                Console.WriteLine("👤 Adding test hero...");
                var insertCommand = new Microsoft.Data.Sqlite.SqliteCommand(@"
            INSERT INTO Heroes (Username, Password, Email) 
            VALUES ('testhero', 'test123', 'test@example.com')", connection);

                int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"✅ Test hero added. Rows affected: {rowsAffected}");

                // Verify the table and data
                Console.WriteLine("🔍 Verifying database...");
                var verifyCommand = new Microsoft.Data.Sqlite.SqliteCommand("SELECT COUNT(*) FROM Heroes", connection);
                var count = Convert.ToInt32(await verifyCommand.ExecuteScalarAsync());
                Console.WriteLine($"✅ Verification: Found {count} heroes in database");

                Console.WriteLine("🎉 DEBUG: Database creation SUCCESSFUL!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG FAILED: {ex.Message}");
                Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        // Add this to Program.cs for complete reset
        public static async Task CompleteDatabaseRebuild()
        {
            try
            {
                string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");

                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    Console.WriteLine("🗑️ Deleted old database");
                    await Task.Delay(1000);
                }

                Console.WriteLine("🏗️ Building complete database schema...");

                using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
                await connection.OpenAsync();

                // Create complete Heroes table
                string heroesSql = @"
            CREATE TABLE Heroes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL,
                Email TEXT NOT NULL,
                Phone TEXT,
                Level INTEGER DEFAULT 1,
                Experience INTEGER DEFAULT 0,
                Class TEXT DEFAULT 'Adventurer',
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            )";

                using var heroesCommand = new Microsoft.Data.Sqlite.SqliteCommand(heroesSql, connection);
                await heroesCommand.ExecuteNonQueryAsync();
                Console.WriteLine("✅ Heroes table created with all columns");

                // Add test hero
                string insertSql = @"
            INSERT INTO Heroes (Username, Password, Email, Phone) 
            VALUES ('testhero', 'test123', 'test@example.com', '+1234567890')";

                using var insertCommand = new Microsoft.Data.Sqlite.SqliteCommand(insertSql, connection);
                await insertCommand.ExecuteNonQueryAsync();
                Console.WriteLine("✅ Test hero added");

                Console.WriteLine("🎉 Database rebuilt successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database rebuild failed: {ex.Message}");
                throw;
            }
        }


    }
}