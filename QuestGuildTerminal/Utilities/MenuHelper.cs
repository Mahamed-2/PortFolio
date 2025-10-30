// Utilities/MenuHelper.cs
using System;

namespace QuestGuildTerminal
{
    public static class MenuHelper
    {
        public static void DisplayHeader(string title)
        {
            Console.Clear();
            Console.WriteLine("🏰 " + new string('═', 50));
            Console.WriteLine($"   {title}");
            Console.WriteLine("🏰 " + new string('═', 50) + "\n");
        }

        public static void DisplaySuccess(string message)
        {
            Console.WriteLine($"\n✅ {message}");
            PressAnyKey();
        }

      
        public static void DisplayError(string message)
        {
            Console.WriteLine($"\n❌ {message}");
            PressAnyKey();
        }

        public static void DisplayWarning(string message)
        {
            Console.WriteLine($"\n⚠️ {message}");
        }

        public static void PressAnyKey()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public static string ReadPassword()
        {
            var password = "";
            ConsoleKeyInfo key;// capture  each key press

            // while to do this until enter is pressed
            do
            {
                key = Console.ReadKey(true);// reads a key without displaying it (true = hide input)
                
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)// handle backspace and remove last char
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);
            
            Console.WriteLine();
            return password;
        }

        public static DateTime ReadFutureDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine(), out var date) && date > DateTime.Now)
                {
                    return date;
                }
                DisplayError("Please enter a valid future date (YYYY-MM-DD):");
            }
        }

        public static Priority ReadPriority()
        {
            Console.WriteLine("\nSelect Priority:");
            Console.WriteLine("1. Low");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. High");
            
            while (true)
            {
                Console.Write("Enter choice (1-3): ");
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1": return Priority.Low;
                    case "2": return Priority.Medium;
                    case "3": return Priority.High;
                    default:
                        DisplayError("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }
            }
        }
    }
}