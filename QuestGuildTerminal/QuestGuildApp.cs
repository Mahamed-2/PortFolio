using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using QuestGuildTerminal.Services;

namespace QuestGuildTerminal
{
    public class QuestGuildApp
    {
        private readonly IAuthenticator _authenticator;
        private readonly INotificationService _notificationService;
        private readonly IGuildAdvisorAI _guildAdvisor;
        private readonly IQuestManager _questManager;
        private readonly IMusicService _musicService;
        private Hero _currentHero;

        public QuestGuildApp(string geminiApiKey = null)
        {
            _authenticator = new Authenticator();
            _notificationService = new NotificationService();
            _guildAdvisor = new EnhancedGuildAdvisorAI();
            _questManager = new QuestManager();
            _musicService = new SimpleLoopingMusicService();
            

            Console.WriteLine("🚀 Quest Guild Terminal Started!");
            Console.WriteLine("💾 Using database authentication");
        }
        // In QuestGuildApp.cs - Update StartBackgroundMusic method
        private async Task StartBackgroundMusic()
        {
            try
            {
                string musicPath = Path.Combine(Directory.GetCurrentDirectory(), "Huntrx.mp3");

                Console.WriteLine($"🔍 Looking for music file at: {musicPath}");
                Console.WriteLine($"📁 File exists: {File.Exists(musicPath)}");

                if (File.Exists(musicPath))
                {
                    Console.WriteLine("🎵 Starting background music...");
                    await _musicService.PlayBackgroundMusicAsync(musicPath);
                    Console.WriteLine("✅ Music playback initiated");
                }
                else
                {
                    Console.WriteLine("❌ Huntrx.mp3 not found in application directory!");
                    Console.WriteLine("💡 Please make sure the file is in the same folder as your application");
                    Console.WriteLine("💡 Current directory: " + Directory.GetCurrentDirectory());

                    // List files in current directory to help debugging
                    Console.WriteLine("📂 Files in current directory:");
                    try
                    {
                        var files = Directory.GetFiles(Directory.GetCurrentDirectory());
                        foreach (var file in files)
                        {
                            Console.WriteLine($"   - {Path.GetFileName(file)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   Could not list files: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🎵 Music error: {ex.Message}");
            }
        }



        public async void Run()
        {
            // Start background music
            await StartBackgroundMusic();
            
            while (true)
            {
                if (_currentHero == null)
                {
                    ShowMainMenu();
                }
                else
                {
                    ShowHeroMenu();
                }
            }
        }

        
        // ... rest of your methods remain exactly the same



private async Task RegisterHeroAsync()
{
    MenuHelper.DisplayHeader("Hero Registration");

    try
    {
        Console.Write("Enter your hero name: ");
        var username = Console.ReadLine();

        Console.Write("Enter your password: ");
        var password = MenuHelper.ReadPassword();

        Console.Write("Enter your email (for guild communications): ");
        var email = Console.ReadLine();

        Console.Write("Enter your phone (for urgent alerts): ");
        var phone = Console.ReadLine();

        var hero = new Hero(username, password, email, phone);

        if (await _authenticator.RegisterAsync(hero))
        {
            MenuHelper.DisplaySuccess($"Hero {username} successfully registered with the guild!");
        }
        else
        {
            MenuHelper.DisplayError("A hero with that name already exists in our records.");
        }
    }
    catch (ArgumentException ex)
    {
        MenuHelper.DisplayError(ex.Message);
    }
}

    private async Task LoginHeroAsync()
    {
        MenuHelper.DisplayHeader("Hero Login");

        Console.Write("Hero name: ");
        var username = Console.ReadLine();

        Console.Write("Password: ");
        var password = MenuHelper.ReadPassword();

        var hero = await _authenticator.LoginAsync(username, password);

        if (hero != null)
        {
            Console.Write("\nEnter the 2FA code sent to you: ");
            var code = Console.ReadLine();

            if (_authenticator.Verify2FA(code))
            {
                _currentHero = hero; // This fixes the CS0649 error
                MenuHelper.DisplaySuccess($"Welcome back, {hero.Username}! The guild is honored by your presence.");
            }
            else
            {
                MenuHelper.DisplayError("Invalid 2FA code. Access denied.");
            }
        }
        else
        {
            MenuHelper.DisplayError("Invalid hero name or password.");
        }
    }
       

        private void ShowMainMenu()
        {
            MenuHelper.DisplayHeader("Quest Guild Terminal - Main Gate");
            Console.WriteLine("1. Register New Hero");
            Console.WriteLine("2. Login Hero");
            Console.WriteLine("3. Music Controls");
            Console.WriteLine("4. Exit Guild");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RegisterHeroAsync().Wait();
                    break;
                case "2":
                    LoginHeroAsync().Wait();
                    break;
                case "3":
                    ShowMusicControls();
                    break;
                case "4":
                    ExitApplication();
                    break;
                default:
                    MenuHelper.DisplayError("Invalid choice. Please try again.");
                    break;
            }
        }
             private void ShowMusicControls()
        {
            MenuHelper.DisplayHeader("Music Controls");
            Console.WriteLine($"🎵 Music Status: {(_musicService.IsPlaying ? "Playing" : "Stopped")}");
            Console.WriteLine($"🔊 Volume: {_musicService.Volume * 100}%");
            Console.WriteLine("\n1. Play Music");
            Console.WriteLine("2. Pause Music");
            Console.WriteLine("3. Stop Music");
            Console.WriteLine("4. Increase Volume");
            Console.WriteLine("5. Decrease Volume");
            Console.WriteLine("6. Back to Main Menu");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    _musicService.Resume();
                    MenuHelper.DisplaySuccess("Music resumed!");
                    break;
                case "2":
                    _musicService.Pause();
                    MenuHelper.DisplaySuccess("Music paused!");
                    break;
                case "3":
                    _musicService.Stop();
                    MenuHelper.DisplaySuccess("Music stopped!");
                    break;
                case "4":
                    _musicService.Volume += 0.1f;
                    MenuHelper.DisplaySuccess($"Volume increased to {_musicService.Volume * 100}%");
                    break;
                case "5":
                    _musicService.Volume -= 0.1f;
                    MenuHelper.DisplaySuccess($"Volume decreased to {_musicService.Volume * 100}%");
                    break;
                case "6":
                    return;
                default:
                    MenuHelper.DisplayError("Invalid choice.");
                    break;
            }
            
            Thread.Sleep(1000); // Brief pause to see the message
        }

        private void ExitApplication()
        {
            _musicService?.Stop();
            _musicService?.Dispose();
            Console.WriteLine("\n🏰 Farewell, brave soul! May we meet again on your next adventure! 🏰");
            Environment.Exit(0);
        }
        private void ShowHeroMenu()
        {
            MenuHelper.DisplayHeader($"Hero's Quarters - Welcome, {_currentHero.Username}!");
            Console.WriteLine("1. Add New Quest");
            Console.WriteLine("2. View All Quests");
            Console.WriteLine("3. Update/Complete Quest");
            Console.WriteLine("4. Request Guild Advisor Help");
            Console.WriteLine("5. Show Guild Report");
            Console.WriteLine("6. Check Deadline Notifications");
            Console.WriteLine("7. Logout");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddQuestAsync().Wait();
                    break;
                case "2":
                    ViewAllQuests();
                    break;
                case "3":
                    UpdateCompleteQuest();
                    break;
                case "4":
                    RequestAdvisorHelpAsync().Wait();
                    break;
                case "5":
                    ShowGuildReport();
                    break;
                case "6":
                    CheckNotificationsAsync().Wait();
                    break;
                case "7":
                    Logout();
                    break;
                default:
                    MenuHelper.DisplayError("Invalid choice. Please try again.");
                    break;
            }
        }
        private async Task AddQuestAsync()
{
    MenuHelper.DisplayHeader("Add New Quest");

    Console.Write("Enter quest title: ");
    var title = Console.ReadLine();

    Console.WriteLine("\nWould you like AI to generate a quest description? (y/n)");
    var useAI = Console.ReadLine().ToLower() == "y";

    string description;

    if (useAI)
    {
        Console.WriteLine("\n🎭 Guild Advisor is crafting your quest description...");
        description = await _guildAdvisor.GenerateQuestDescriptionAsync(title);
        Console.WriteLine($"\nGenerated Description: {description}");
    }
    else
    {
        Console.Write("Enter quest description: ");
        description = Console.ReadLine();
    }

    var dueDate = MenuHelper.ReadFutureDate("Enter due date (YYYY-MM-DD): ");

    Console.WriteLine("\nWould you like AI to suggest priority? (y/n)");
    var suggestPriority = Console.ReadLine().ToLower() == "y";

    Priority priority;

    if (suggestPriority)
    {
        priority = await _guildAdvisor.SuggestPriorityAsync(title, dueDate);
        Console.WriteLine($"\n🎭 Guild Advisor suggests: {priority} Priority");
    }
    else
    {
        priority = MenuHelper.ReadPriority();
    }

    var quest = new Quest(title, description, dueDate, priority, _currentHero?.Id ?? 0);
    _questManager.AddQuest(quest);

    MenuHelper.DisplaySuccess($"Quest '{title}' has been added to your journal!");
}

private void ViewAllQuests()
{
    MenuHelper.DisplayHeader("Your Quest Journal");

    var quests = _questManager.GetAllQuests();

    if (quests.Count == 0)
    {
        Console.WriteLine("Your quest journal is empty. Time for an adventure!");
        MenuHelper.PressAnyKey();
        return;
    }

    Console.WriteLine("=== ACTIVE QUESTS ===");
    var activeQuests = _questManager.GetActiveQuests();
    if (activeQuests.Count > 0)
    {
        foreach (var quest in activeQuests)
        {
            Console.WriteLine(quest);
            Console.WriteLine();
        }
    }
    else
    {
        Console.WriteLine("No active quests.\n");
    }

    Console.WriteLine("=== COMPLETED QUESTS ===");
    var completedQuests = _questManager.GetCompletedQuests();
    if (completedQuests.Count > 0)
    {
        foreach (var quest in completedQuests)
        {
            Console.WriteLine(quest);
            Console.WriteLine();
        }
    }
    else
    {
        Console.WriteLine("No completed quests yet.\n");
    }

    MenuHelper.PressAnyKey();
}

private void UpdateCompleteQuest()
{
    MenuHelper.DisplayHeader("Update or Complete Quest");
    
    var activeQuests = _questManager.GetActiveQuests();
    if (activeQuests.Count == 0)
    {
        Console.WriteLine("No active quests to update or complete.");
        MenuHelper.PressAnyKey();
        return;
    }
    
    Console.WriteLine("Select a quest to update or complete:\n");
    for (int i = 0; i < activeQuests.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {activeQuests[i].Title} (Due: {activeQuests[i].DueDate:yyyy-MM-dd})");
    }
    
    Console.Write("\nEnter quest number: ");
    if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= activeQuests.Count)
    {
        var quest = activeQuests[choice - 1];
        
        Console.WriteLine($"\nSelected: {quest}");
        Console.WriteLine("\n1. Mark as Complete");
        Console.WriteLine("2. Update Quest Details");
        Console.Write("\nEnter choice: ");
        
        var action = Console.ReadLine();
        switch (action)
        {
            case "1":
                if (_questManager.CompleteQuest(quest.Id.ToString()))
                {
                    MenuHelper.DisplaySuccess($"Quest '{quest.Title}' marked as complete! The guild honors your achievement!");
                }
                break;
            case "2":
                UpdateQuestDetails(quest);
                break;
            default:
                MenuHelper.DisplayError("Invalid choice.");
                break;
        }
    }
    else
    {
        MenuHelper.DisplayError("Invalid quest selection.");
    }
}

private void UpdateQuestDetails(Quest quest)
{
    Console.Write($"Enter new title [{quest.Title}]: ");
    var newTitle = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newTitle)) newTitle = quest.Title;
    
    Console.Write($"Enter new description [{quest.Description}]: ");
    var newDescription = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newDescription)) newDescription = quest.Description;
    
    var newDueDate = MenuHelper.ReadFutureDate($"Enter new due date [{quest.DueDate:yyyy-MM-dd}]: ");
    var newPriority = MenuHelper.ReadPriority();
    
    if (_questManager.UpdateQuest(quest.Id.ToString(), newTitle, newDescription, newDueDate, newPriority))
    {
        MenuHelper.DisplaySuccess("Quest updated successfully!");
    }
}

private async Task RequestAdvisorHelpAsync()
{
    MenuHelper.DisplayHeader("Guild Advisor Assistance");

    Console.WriteLine("1. Generate Quest Summary");
    Console.WriteLine("2. Get AI Quest Description Help");
    Console.Write("\nEnter your choice: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            Console.WriteLine("\n🎭 Guild Advisor is analyzing your quests...");
            var summary = await _guildAdvisor.SummarizeQuestsAsync(_questManager);
            Console.WriteLine($"\n{summary}");
            break;
        case "2":
            Console.Write("\nEnter quest title for AI description: ");
            var title = Console.ReadLine();
            Console.WriteLine("\n🎭 Guild Advisor is crafting your quest...");
            var description = await _guildAdvisor.GenerateQuestDescriptionAsync(title);
            Console.WriteLine($"\nGenerated Quest Description:\n{description}");
            break;
        default:
            MenuHelper.DisplayError("Invalid choice.");
            return;
    }
    
    MenuHelper.PressAnyKey();
}

private void ShowGuildReport()
{
    MenuHelper.DisplayHeader("Guild Report");

    Console.WriteLine(_questManager.GetQuestSummary());

    var nearDeadline = _questManager.GetQuestsNearDeadline();
    if (nearDeadline.Count > 0)
    {
        Console.WriteLine("\n⚠️ QUESTS NEEDING IMMEDIATE ATTENTION:");
        foreach (var quest in nearDeadline)
        {
            Console.WriteLine($"   • {quest.Title} (Due: {quest.DueDate:yyyy-MM-dd HH:mm})");
        }
    }

    MenuHelper.PressAnyKey();
}

        private async Task CheckNotificationsAsync()
        {
            MenuHelper.DisplayHeader("Guild Alerts");
            Console.WriteLine("Checking for deadline notifications...\n");

            await _notificationService.CheckDeadlineNotificationsAsync(_questManager, _currentHero.Email);
            MenuHelper.PressAnyKey();
        }
        // Add this method to QuestGuildApp.cs
        private async Task DebugMusicFile()
        {
            string musicPath = Path.Combine(Directory.GetCurrentDirectory(), "Huntrx.mp3");
            Console.WriteLine($"🔍 Music file path: {musicPath}");
            Console.WriteLine($"📁 File exists: {File.Exists(musicPath)}");

            if (File.Exists(musicPath))
            {
                FileInfo fileInfo = new FileInfo(musicPath);
                Console.WriteLine($"📊 File size: {fileInfo.Length} bytes");
                Console.WriteLine($"📊 File extension: {Path.GetExtension(musicPath)}");
            }
            else
            {
                Console.WriteLine("❌ Huntrx.mp3 not found in application directory!");
                Console.WriteLine("💡 Current directory: " + Directory.GetCurrentDirectory());
                Console.WriteLine("💡 Please make sure Huntrx.mp3 is in the same folder as your .exe file");
            }
        }


private void Logout()
{
    _currentHero = null;
    MenuHelper.DisplaySuccess("You have been logged out safely. Until next time, hero!");
}
    


        // ... all other methods remain exactly the same as in your original file
    }
}