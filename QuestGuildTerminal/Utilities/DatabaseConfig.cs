// Utilities/DatabaseConfig.cs
using System;
using System.IO;

namespace QuestGuildTerminal
{
    public static class DatabaseConfig
    {
        public static string ConnectionString 
        { 
            get 
            {
                // Use the current directory directly, not nested QuestGuildTerminal folder
                string databasePath = Path.Combine(Directory.GetCurrentDirectory(), "questguild.db");
                Console.WriteLine($"🔍 Database path: {databasePath}");
                return $"Data Source={databasePath}";
            }
        }
    }
}