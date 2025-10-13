// Services/DatabaseInitializer.cs
using QuestGuildTerminal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync()
        {
            try
            {
                Console.WriteLine("🗄️ Initializing database...");
                
                using var context = new QuestGuildContext();
                
                // This will create the database if it doesn't exist
                await context.Database.MigrateAsync();
                
                Console.WriteLine("✅ Database initialized successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
                throw;
            }
        }
    }
}