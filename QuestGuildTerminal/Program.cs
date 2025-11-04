

using Spectre.Console;
using QuestGuildTerminal.Data; 



namespace QuestGuildTerminal
{
    class Program
    {
    static async Task Main(string[] args)
    {
        
        AnsiConsole.Write(
            new FigletText("QUEST GUILD")
                .Color(Color.Purple));
        AnsiConsole.Write(
            new FigletText("TERMINAL v1.0")
                .Color(Color.Blue));
        
        AnsiConsole.MarkupLine("[bold yellow]═══════════════════════════════════════[/]");
        AnsiConsole.MarkupLine("[green]🎮 1987 TEXT-BASED ADVENTURE SYSTEM 🎮[/]");
        AnsiConsole.MarkupLine("[bold yellow]═══════════════════════════════════════[/]");
        
        AnsiConsole.MarkupLine("\n[cyan]Initializing guild systems...[/]");
        
        await Task.Delay(2000);
        
        try
        {
            await DatabaseInitializer.InitializeDatabaseAsync();
            await DatabaseInitializer.DebugDatabaseSchema();

            var app = new QuestGuildApp();
            await app.Run();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]💥 SYSTEM FAILURE: {ex.Message}[/]");
            AnsiConsole.MarkupLine("[yellow]💡 Try: dotnet run -- --rebuild[/]");
            Console.ReadKey();
        }
    }



        
        

  }
}
