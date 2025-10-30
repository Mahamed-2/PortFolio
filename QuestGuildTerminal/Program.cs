// Program.cs

using System.Text;

using QuestGuildTerminal.Data; 



namespace QuestGuildTerminal
{
    class Program
    {
        // In Program.cs 
        static async Task Main(string[] args)
        {
            Console.WriteLine("🏰 Welcome to the Quest Guild Terminal! 🏰");

            // Force complete rebuild and initialization of the database
            if (args.Length > 0 && args[0] == "--rebuild")
            {
                
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



       
       

    }
}
