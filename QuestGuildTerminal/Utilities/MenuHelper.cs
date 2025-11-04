using Spectre.Console; // ADD THIS
using System;

namespace QuestGuildTerminal
{
    public static class MenuHelper
    {
        // UPDATE: Use AnsiConsole for displays
        public static void DisplayHeader(string title)
        {
            Console.Clear();
            var panel = new Panel($"[bold cyan]{title}[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Padding = new Padding(1, 1, 1, 1)
            };
            AnsiConsole.Write(panel);
        }

        public static void DisplaySuccess(string message)
        {
            AnsiConsole.MarkupLine($"[bold green]✅ {message}[/]");
        }

        public static void DisplayError(string message)
        {
            AnsiConsole.MarkupLine($"[bold red]❌ {message}[/]");
        }

        public static void PressAnyKey()
        {
            AnsiConsole.MarkupLine("\n[cyan]Press any key to continue...[/]");
            Console.ReadKey();
        }

        // Keep your existing ReadPassword, ReadFutureDate, ReadPriority methods
        public static string ReadPassword()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>("Enter password:")
                    .PromptStyle("red")
                    .Secret()
            );
        }

        public static DateTime ReadFutureDate(string prompt)
        {
            AnsiConsole.MarkupLine($"[cyan]{prompt}[/]");
            // Your existing date reading logic
            return DateTime.Now.AddDays(7); // Example
        }

        public static Priority ReadPriority()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<Priority>()
                    .Title("[cyan]Select priority:[/]")
                    .AddChoices(Enum.GetValues<Priority>())
            );
        }
    }
}