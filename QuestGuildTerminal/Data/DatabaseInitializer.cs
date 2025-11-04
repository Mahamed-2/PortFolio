// Data/DatabaseInitializer.cs
using System;
using System.IO;
using System.Threading.Tasks;

namespace QuestGuildTerminal.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync()
        {
            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");
            Console.WriteLine($"🔍 Initializing database at: {dbPath}");

            try
            {
                bool isNewDatabase = !File.Exists(dbPath);
                
                if (isNewDatabase)
                {
                    Console.WriteLine("🆕 Creating new database...");
                    // Create empty database file
                    File.WriteAllBytes(dbPath, new byte[0]);
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
                        // Create all tables for new database
                        await CreateHeroesTable(connection);
                        await CreateQuestsTable(connection);
                        await AddTestData(connection);
                        Console.WriteLine("🎉 New database created successfully!");
                    }
                    else
                    {
                        // Update existing database schema
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

        // Data/DatabaseInitializer.cs - Update the UpdateDatabaseSchema method// Data/DatabaseInitializer.cs - Update UpdateDatabaseSchema method
        private static async Task UpdateDatabaseSchema(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            try
            {
                // Define all required columns for Heroes table
                var requiredColumns = new Dictionary<string, string>
        {
            { "Phone", "TEXT" },
            { "Level", "INTEGER DEFAULT 1" },
            { "Experience", "INTEGER DEFAULT 0" },
            { "Class", "TEXT DEFAULT 'Adventurer'" },
            { "CreatedAt", "DATETIME DEFAULT CURRENT_TIMESTAMP" }
        };

                foreach (var column in requiredColumns)
                {
                    bool columnExists = await CheckColumnExists(connection, "Heroes", column.Key);

                    if (!columnExists)
                    {
                        Console.WriteLine($"🔧 Adding {column.Key} column to Heroes table...");
                        string addColumnSql = $"ALTER TABLE Heroes ADD COLUMN {column.Key} {column.Value}";
                        using var command = new Microsoft.Data.Sqlite.SqliteCommand(addColumnSql, connection);
                        await command.ExecuteNonQueryAsync();
                        Console.WriteLine($"✅ Added {column.Key} column to Heroes table");
                    }
                    else
                    {
                        Console.WriteLine($"✅ {column.Key} column already exists in Heroes table");
                    }
                }

                // Check if Quests table exists
                bool questsTableExists = await CheckTableExists(connection, "Quests");

                if (!questsTableExists)
                {
                    Console.WriteLine("🔧 Creating Quests table...");
                    await CreateQuestsTable(connection);
                    Console.WriteLine("✅ Quests table created");
                }
                else
                {
                    Console.WriteLine("✅ Quests table already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Schema update failed: {ex.Message}");
                throw;
            }
        }


// Add this method to check if a table exists
private static async Task<bool> CheckTableExists(Microsoft.Data.Sqlite.SqliteConnection connection, string tableName)
{
    try
    {
        string sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
        using var command = new Microsoft.Data.Sqlite.SqliteCommand(sql, connection);
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Could not check if table {tableName} exists: {ex.Message}");
        return false;
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
                // Only add test hero if it doesn't exist
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
        // Add this method to DatabaseInitializer.cs
        public static async Task DebugDatabaseSchema()
        {
            try
            {
                string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");
                using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
                await connection.OpenAsync();

                Console.WriteLine("🔍 Debugging Heroes table schema:");

                // Get all columns in Heroes table
                string schemaQuery = "PRAGMA table_info(Heroes)";
                using var command = new Microsoft.Data.Sqlite.SqliteCommand(schemaQuery, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"   Column: {reader["name"]} | Type: {reader["type"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Schema debug failed: {ex.Message}");
            }
        }

    }
}