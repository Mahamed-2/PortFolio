using QuestGuildTerminal.Services;

using Spectre.Console;

namespace QuestGuildTerminal
{
    public class QuestGuildApp
    {  
        private readonly IAuthenticator _authenticator;
        private readonly INotificationService _notificationService;
        private readonly IGuildAdvisorAI _guildAdvisor;
        private readonly IQuestManager _questManager;
        private readonly IMusicService _musicService;
        private readonly GameManager _gameManager;
        private Hero _currentHero;

        public QuestGuildApp(string geminiApiKey = null)
         {
            _notificationService = new NotificationService(); // Create this first
            _authenticator = new Authenticator(_notificationService); // Pass to authenticator
            _guildAdvisor = new EnhancedGuildAdvisorAI();
            _musicService = new SimpleLoopingMusicService();
            _gameManager = new GameManager(_musicService);
            _questManager = new QuestManager(_gameManager);
        
            
            // Spectre.Console welcome banner
            AnsiConsole.Write(new FigletText("Quest Guild").Color(Color.Gold3));
            AnsiConsole.MarkupLine("[bold green]🚀 Quest Guild Terminal Started![/]");
        }

        private async Task StartBackgroundMusic()
        {
            try
            {
                string musicPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Huntrx.mp3");
                if (File.Exists(musicPath))
                {
                    await _musicService.PlayBackgroundMusicAsync(musicPath);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Music error: {ex.Message}[/]");
            }
        }

        public async Task Run()
        {
            await StartBackgroundMusic();
            
            while (true)
            {
                if (_currentHero == null)
                {
                    await ShowSafeMainMenu();
                }
                else
                {
                    await ShowSafeHeroMenu(); // Changed to async
                }
                
                await Task.Delay(100); // Prevent tight loop
            }
        }

        private async Task ShowSafeMainMenu()
        {
            AnsiConsole.Clear();
            
            // Header panel
            var headerPanel = new Panel("[bold gold3]⚔️ QUEST GUILD TERMINAL ⚔️[/]")
                .Border(BoxBorder.Double)
                .BorderColor(Color.Gold3)
                .Padding(1, 1)
                .HeaderAlignment(Justify.Center);
            
            AnsiConsole.Write(headerPanel);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]Choose your path:[/]")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "🎯 Register New Hero",
                        "🔐 Login Hero", 
                        "🎵 Music Controls",
                        "🚪 Exit Guild"
                    }));

            switch (choice)
            {
                case "🎯 Register New Hero":
                    await SafeRegisterHeroAsync();
                    break;
                case "🔐 Login Hero":
                    await SafeLoginHeroAsync();
                    break;
                case "🎵 Music Controls":
                    await ShowSafeMusicControls();
                    break;
                case "🚪 Exit Guild":
                    if (AnsiConsole.Confirm("[red]Are you sure you want to leave the guild?[/]"))
                    {
                        AnsiConsole.MarkupLine("[yellow]Farewell, adventurer![/]");
                        await ShutdownMusicAsync();
                        Environment.Exit(0);
                    }
                    break;
            }
        }

        private async Task ShutdownMusicAsync()
        {
            try
            {
                if (_musicService != null)
                {
                    AnsiConsole.MarkupLine("[yellow]🎵 Stopping background music...[/]");
                    _musicService.Stop();
                    AnsiConsole.MarkupLine("[green]✅ Music stopped[/]");
                    await Task.Delay(500); // Brief pause to show message
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error stopping music: {ex.Message}[/]");
            }
        }

        private async Task ShowSafeHeroMenu() // Changed to async Task
        {
             while (_currentHero != null)
    {
        AnsiConsole.Clear();
        
        // Check for urgent notifications
        await CheckBackgroundNotifications();
        
        // Rest of your existing menu code...
        var heroPanel = new Panel($"[bold gold3]🏰 HERO'S QUARTERS[/]\n\nWelcome, [bold cyan]{_currentHero.Username}[/]!")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Cyan1)
            .Padding(1, 1);
        
        AnsiConsole.Write(heroPanel);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold cyan]What would you like to do?[/]")
                        .PageSize(12)
                        .AddChoices(new[] {
                            "📝 Add New Quest",
                            "📖 View All Quests", 
                            "✏️ Update/Complete Quest",
                            "🎮 Complete Quest with Game Challenge",
                            "🧠 Request Guild Advisor Help",
                            "📊 Show Guild Report",
                            "🔔 Check Deadline Notifications",
                            "🎯 View Game Challenges",
                            "🚪 Logout"
                        }));

                switch (choice)
                {
                    case "📝 Add New Quest":
                        await SafeAddQuestAsync(); // Fixed: await instead of .Wait()
                        break;
                    case "📖 View All Quests":
                        SafeViewAllQuests();
                        break;
                    case "✏️ Update/Complete Quest":
                        await SafeUpdateCompleteQuest();
                        break;
                    case "🎮 Complete Quest with Game Challenge":
                        await SafeCompleteQuestWithGameAsync(); // Fixed: await instead of .Wait()
                        break;
                    case "🧠 Request Guild Advisor Help":
                        await SafeRequestAdvisorHelpAsync(); // Fixed: await instead of .Wait()
                        break;
                    case "📊 Show Guild Report":
                        SafeShowGuildReport();
                        break;
                    case "🔔 Check Deadline Notifications":
                        await SafeCheckNotificationsAsync(); // Fixed: await instead of .Wait()
                        break;
                    case "🎯 View Game Challenges":
                        SafeViewGameChallenges();
                        break;
                    case "🚪 Logout":
                        await SafeLogoutAsync(); // Changed to async version
                        return;
                }
                
                await Task.Delay(100); // Prevent tight loop
            }
        }

        private async Task SafeLogoutAsync() // New async logout method
        {
            AnsiConsole.MarkupLine($"[yellow]👋 Goodbye, {_currentHero?.Username}![/]");
            
            await AnsiConsole.Status()
                .StartAsync("Logging out...", async ctx => 
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("yellow"));
                    await Task.Delay(2000);
                });
            
            await ShutdownMusicAsync(); // Stop music on logout
            _currentHero = null;
        }

private async Task SafeLoginHeroAsync()
{
    AnsiConsole.Clear();
    
    var panel = new Panel("[bold gold3]🔐 HERO LOGIN[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Blue);
    
    AnsiConsole.Write(panel);

    var username = AnsiConsole.Ask<string>("[cyan]Hero Name:[/]");
    var password = AnsiConsole.Prompt(
        new TextPrompt<string>("[cyan]Password:[/]")
            .PromptStyle("red")
            .Secret());

    // Attempt login - returns null if credentials valid but 2FA pending
    var loginResult = await _authenticator.LoginAsync(username, password);

    if (loginResult == null && _authenticator.HasPending2FA(username))
    {
        // Valid credentials, 2FA sent
        AnsiConsole.MarkupLine("[green]✅ 2FA code sent to your registered contact[/]");
        
        var code = AnsiConsole.Ask<string>("[cyan]Enter the 2FA code:[/]");

        if (_authenticator.Verify2FA(code))
        {
            // Get the stored hero after successful 2FA
            _currentHero = _authenticator.GetPendingHero(username);
            
            if (_currentHero != null)
            {
                AnsiConsole.MarkupLine($"[bold green]✅ Welcome back, {_currentHero.Username}![/]");
                AnsiConsole.MarkupLine("[yellow]Entering hero quarters...[/]");
                
                await AnsiConsole.Status()
                    .StartAsync("Loading...", async ctx => 
                    {
                        ctx.Spinner(Spinner.Known.Star);
                        ctx.SpinnerStyle(Style.Parse("green"));
                        await Task.Delay(2000);
                    });
                return;
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]❌ Invalid 2FA code. Access denied.[/]");
        }
    }
    else
    {
        AnsiConsole.MarkupLine("[red]❌ Invalid hero name or password.[/]");
    }
    
    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
    SafeReadKey();
}

        private async Task SafeRegisterHeroAsync()
        {
            AnsiConsole.Clear();
            
            var panel = new Panel("[bold gold3]🎯 HERO REGISTRATION[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Green);
            
            AnsiConsole.Write(panel);

            var username = AnsiConsole.Ask<string>("[cyan]Enter your hero name:[/]");
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[cyan]Enter your password:[/]")
                    .PromptStyle("red")
                    .Secret());
            var email = AnsiConsole.Ask<string>("[cyan]Enter your email:[/]");
            var phone = AnsiConsole.Ask<string>("[cyan]Enter your phone:[/]");

            var hero = new Hero(username, password, email, phone);

            // Show progress during registration
            await AnsiConsole.Progress()
                .StartAsync(async ctx => 
                {
                    var task1 = ctx.AddTask("[green]Creating hero profile[/]");
                    var task2 = ctx.AddTask("[blue]Setting up quest journal[/]");
                    
                    while (!ctx.IsFinished)
                    {
                        task1.Increment(1.5);
                        task2.Increment(1.0);
                        await Task.Delay(50);
                    }
                });

            if (await _authenticator.RegisterAsync(hero))
            {
                AnsiConsole.MarkupLine($"[bold green]✅ Hero {username} successfully registered![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ A hero with that name already exists.[/]");
            }
            
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            SafeReadKey();
        }

       private async Task SafeAddQuestAsync()
{
    AnsiConsole.Clear();
    
    var panel = new Panel("[bold gold3]📝 ADD NEW QUEST[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Orange1);
    
    AnsiConsole.Write(panel);

    var title = AnsiConsole.Ask<string>("[cyan]Quest title:[/]");
    
    var useAI = AnsiConsole.Confirm("[cyan]Use AI for description?[/]");
    string description = string.Empty;

    if (useAI)
    {
        AnsiConsole.MarkupLine("[yellow]Generating description with AI...[/]");
        
        description = await AnsiConsole.Status()
            .StartAsync("Consulting the Guild Advisor...", async ctx => 
            {
                ctx.Spinner(Spinner.Known.Earth);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                return await _guildAdvisor.GenerateQuestDescriptionAsync(title);
            });
        
        AnsiConsole.MarkupLine($"[green]Generated: {description}[/]");
    }
    else
    {
        description = AnsiConsole.Ask<string>("[cyan]Quest description:[/]");
    }

    var days = AnsiConsole.Prompt(
        new TextPrompt<int>("[cyan]Due in how many days?[/]")
            .ValidationErrorMessage("[red]That's not a valid number[/]")
            .Validate(days => 
            {
                return days switch
                {
                    < 0 => ValidationResult.Error("[red]Cannot be negative[/]"),
                    > 365 => ValidationResult.Error("[red]That's too far in the future[/]"),
                    _ => ValidationResult.Success()
                };
            }));

    var dueDate = DateTime.Now.AddDays(days);

    var includeGameChallenge = AnsiConsole.Confirm("[cyan]Add Tetris challenge?[/]");

    var quest = new Quest(title, description, dueDate, Priority.Medium, _currentHero?.Id ?? 0);
    _questManager.AddQuest(quest, includeGameChallenge);

    // Send new quest notification
    if (_currentHero != null)
    {
        var heroContact = _currentHero.Email ?? _currentHero.Phone;
        await _notificationService.SendNotificationAsync(
            $"📋 New Quest Registered: '{title}' due on {dueDate:MMM dd}. Good luck, hero!",
            heroContact
        );
    }

    if (includeGameChallenge)
    {
        AnsiConsole.MarkupLine($"[bold green]🎯 Quest '{title}' added with Tetris challenge![/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[bold green]🎯 Quest '{title}' added![/]");
    }
    
    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
    SafeReadKey();
}


        private void SafeViewAllQuests()
        {
            AnsiConsole.Clear();
            
            var panel = new Panel("[bold gold3]📖 YOUR QUEST JOURNAL[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Purple);
            
            AnsiConsole.Write(panel);

            var quests = _questManager.GetAllQuests();

            if (quests.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Your quest journal is empty.[/]");
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);
                table.AddColumn("[bold]Title[/]");
                table.AddColumn("[bold]Description[/]");
                table.AddColumn("[bold]Due Date[/]");
                table.AddColumn("[bold]Status[/]");

                foreach (var quest in quests)
                {
                    var statusColor = quest.IsCompleted ? "green" : "yellow";
                    var statusText = quest.IsCompleted ? "✅ Completed" : "⏳ Pending";
                    
                    // Safe string handling
                    var safeDescription = quest.Description ?? "";
                    var displayDescription = safeDescription.Length > 30 
                        ? safeDescription.Substring(0, 30) + "..." 
                        : safeDescription;
                    
                    table.AddRow(
                        Markup.Escape(quest.Title ?? ""),
                        Markup.Escape(displayDescription),
                        quest.DueDate.ToString("MMM dd"),
                        $"[{statusColor}]{statusText}[/]"
                    );
                }
                
                AnsiConsole.Write(table);
            }
            
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            SafeReadKey();
        }

        // Safe input methods
        private string SafeReadLine()
        {
            try
            {
                return Console.ReadLine() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private void SafeReadKey()
        {
            try
            {
                Console.ReadKey(true);
            }
            catch
            {
                Thread.Sleep(1000);
            }
        }

        private async Task ShowSafeMusicControls()
        {
            AnsiConsole.Clear();
            
            var panel = new Panel("[bold gold3]🎵 MUSIC CONTROLS[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Aqua);
            
            AnsiConsole.Write(panel);

            var status = _musicService.IsPlaying ? "[green]Playing[/]" : "[red]Stopped[/]";
            AnsiConsole.MarkupLine($"Status: {status}");
            AnsiConsole.MarkupLine($"Volume: [blue]{_musicService.Volume * 100}%[/]");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan]Music Controls:[/]")
                    .AddChoices(new[] {
                        "▶️ Play", "⏸️ Pause", "⏹️ Stop", "🔙 Back"
                    }));

            // Basic music control implementation
            try
            {
                switch (choice)
                {
                    case "▶️ Play":
                        try
                        {
                            string musicPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Huntrx.mp3");
                            if (File.Exists(musicPath))
                            {
                                await _musicService.PlayBackgroundMusicAsync(musicPath);
                                AnsiConsole.MarkupLine("[green]Music started[/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("[red]Music file not found.[/]");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"[red]Error starting music: {ex.Message}[/]");
                        }
                        break;
                    case "⏸️ Pause":
                        _musicService.Pause();
                        AnsiConsole.MarkupLine("[yellow]Music paused[/]");
                        break;
                    case "⏹️ Stop":
                        _musicService.Stop();
                        AnsiConsole.MarkupLine("[red]Music stopped[/]");
                        break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Music control error: {ex.Message}[/]");
            }
            
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            SafeReadKey();
        }
   private async Task SafeUpdateCompleteQuest() // Changed to async Task
{
    try
    {
        AnsiConsole.Clear();
        var quests = _questManager.GetAllQuests().Where(q => !q.IsCompleted).ToList();
        
        if (quests.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No pending quests to update.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            SafeReadKey();
            return;
        }

        var questTitles = quests.Select(q => q.Title).ToArray();
        var selectedTitle = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Select quest to update:[/]")
                .AddChoices(questTitles));

        var quest = quests.First(q => q.Title == selectedTitle);
        
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[cyan]What to do with '{selectedTitle}'?[/]")
                .AddChoices(new[] { "Mark as Completed", "Change Due Date", "Cancel" }));

        switch (action)
        {
            case "Mark as Completed":
                quest.IsCompleted = true;
                AnsiConsole.MarkupLine($"[green]✅ Quest '{selectedTitle}' marked as completed![/]");
                
                // Send completion notification
                if (_currentHero != null)
                {
                    var heroContact = _currentHero.Email ?? _currentHero.Phone;
                    await _notificationService.SendNotificationAsync(
                        $"🎉 Quest Completed: '{selectedTitle}'! Great work, hero!",
                        heroContact
                    );
                }
                break;
                
            case "Change Due Date":
                var days = AnsiConsole.Prompt(
                    new TextPrompt<int>("[cyan]Due in how many days?[/]")
                        .DefaultValue(7)
                        .Validate(d => d > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Must be positive[/]")));
                quest.DueDate = DateTime.Now.AddDays(days);
                AnsiConsole.MarkupLine($"[green]📅 Due date updated to {quest.DueDate:MMM dd}[/]");
                break;
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error updating quest: {ex.Message}[/]");
    }
    
    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
    SafeReadKey();
}

private async Task SafeCompleteQuestWithGameAsync()
{
    try
    {
        AnsiConsole.Clear();
    
        var panel = new Panel("[bold gold3]🎮 QUEST GAME CHALLENGE[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Orange1);
    
        AnsiConsole.Write(panel);

        // Get available games
        var availableGames = _gameManager.GetAvailableGames();
    
        if (availableGames.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]❌ No games available![/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            SafeReadKey();
            return;
        }

        // Let user select a game
        var selectedGame = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Choose your game challenge:[/]")
                .PageSize(5)
                .AddChoices(availableGames));

        // Get target level
        var targetLevel = AnsiConsole.Prompt(
            new TextPrompt<int>("[cyan]Enter target level to complete quest:[/]")
                .DefaultValue(3)
                .ValidationErrorMessage("[red]Please enter a valid number[/]")
                .Validate(level => 
                {
                    return level switch
                    {
                        < 1 => ValidationResult.Error("[red]Level must be at least 1[/]"),
                        > 10 => ValidationResult.Error("[red]Level cannot exceed 10[/]"),
                        _ => ValidationResult.Success()
                    };
                }));

        // Show confirmation
        AnsiConsole.MarkupLine($"[yellow]Starting {selectedGame} challenge - Reach Level {targetLevel}[/]");
    
        if (!AnsiConsole.Confirm("[cyan]Ready to begin the challenge?[/]"))
        {
            AnsiConsole.MarkupLine("[yellow]Challenge cancelled.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            SafeReadKey();
            return;
        }

        // Start the game
        AnsiConsole.MarkupLine($"[green]🚀 Launching {selectedGame}...[/]");
    
        var result = await _gameManager.PlayGameAsync(selectedGame, targetLevel);

        // Show game results
        AnsiConsole.Clear();
    
        var resultPanel = new Panel(
            new Markup($"[bold gold3]🎮 GAME RESULTS[/]\n\n" +
                      $"[blue]Game:[/] {selectedGame}\n" +
                      $"[blue]Target Level:[/] {targetLevel}\n" +
                      $"[blue]Final Level:[/] {result.FinalLevel}\n" +
                      $"[blue]Score:[/] {result.Score}\n" +
                      $"[blue]Quest Status:[/] [bold {(result.Success ? "green" : "red")}]{(result.Success ? "COMPLETED" : "FAILED")}[/]"))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(result.Success ? Color.Green : Color.Red),
            Padding = new Padding(2, 1, 2, 1)
        };
    
        AnsiConsole.Write(resultPanel);

        if (result.Success)
        {
            AnsiConsole.MarkupLine("\n[bold green]🎉 Congratulations! You completed the quest challenge![/]");
        
            // Here you would mark the quest as completed in your quest manager
            // For example: _questManager.CompleteQuest(questId);
        }
        else
        {
            AnsiConsole.MarkupLine("\n[bold yellow]💪 The challenge continues! Try again to complete your quest.[/]");
        }
    
        AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
        SafeReadKey();
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]❌ Error starting game: {ex.Message}[/]");
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        SafeReadKey();
    }
}

private async Task SafeRequestAdvisorHelpAsync()
{
    try
    {
        AnsiConsole.Clear();

        var panel = new Panel("[bold gold3]🧠 GUILD ADVISOR[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Aqua);

        AnsiConsole.Write(panel);

        var topic = AnsiConsole.Ask<string>("[cyan]What do you need advice on?[/]");

        AnsiConsole.MarkupLine("[yellow]Consulting the Guild Advisor...[/]");

        var advice = await AnsiConsole.Status()
            .StartAsync("Thinking...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                // Reuse the guild advisor's description generation for advice output
                return await _guildAdvisor.GenerateQuestDescriptionAsync(topic);
            });

        AnsiConsole.MarkupLine($"\n[green]{advice}[/]");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]❌ Advisor error: {ex.Message}[/]");
    }
    AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
    SafeReadKey();
}

private void SafeShowGuildReport()
{
    AnsiConsole.Clear();

    var panel = new Panel("[bold gold3]📊 GUILD REPORT[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Gold3);

    AnsiConsole.Write(panel);

    var quests = _questManager.GetAllQuests();
    var total = quests.Count;
    var completed = quests.Count(q => q.IsCompleted);
    var pending = total - completed;
    var overdue = quests.Count(q => !q.IsCompleted && q.DueDate < DateTime.Now);

    var table = new Table();
    table.Border(TableBorder.Rounded);
    table.AddColumn("[bold]Metric[/]");
    table.AddColumn("[bold]Value[/]");

    table.AddRow("Total Quests", total.ToString());
    table.AddRow("Completed", completed.ToString());
    table.AddRow("Pending", pending.ToString());
    table.AddRow("Overdue", overdue.ToString());

    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
    SafeReadKey();
}

private async Task SafeCheckNotificationsAsync()
{
    AnsiConsole.Clear();

    var panel = new Panel("[bold gold3]🔔 DEADLINE NOTIFICATIONS[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Orange1);

    AnsiConsole.Write(panel);

    try
    {
        // Use the notification service to check deadline notifications
        if (_currentHero != null)
        {
            var heroContact = _currentHero.Email ?? _currentHero.Phone;
            await _notificationService.CheckDeadlineNotificationsAsync(_questManager, heroContact);
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No hero logged in to check notifications for.[/]");
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]❌ Notification error: {ex.Message}[/]");
    }

    AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
    SafeReadKey();
}


private void SafeViewGameChallenges()
{
    AnsiConsole.Clear();

    var panel = new Panel("[bold gold3]🎯 AVAILABLE GAME CHALLENGES[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Purple);

    AnsiConsole.Write(panel);

    var availableGames = _gameManager.GetAvailableGames();

    if (availableGames.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]No game challenges available at the moment.[/]");
    }
    else
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn("[bold]Game[/]");
        table.AddColumn("[bold]Description[/]");
        table.AddColumn("[bold]Challenge[/]");

        foreach (var game in availableGames)
        {
            table.AddRow(
                game,
                "Complete the target level to finish your quest",
                "Reach specified level"
            );
        }
    
        AnsiConsole.Write(table);
    
        AnsiConsole.MarkupLine("\n[grey]Note: Complete a game challenge to mark your quest as completed![/]");
    }

    AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
    SafeReadKey();
}
private async Task CheckBackgroundNotifications()
{
    try
    {
        if (_currentHero != null)
        {
            var heroContact = _currentHero.Email ?? _currentHero.Phone;
            
            // Check for urgent deadlines every time hero enters menu
            var urgentQuests = _questManager.GetAllQuests()
                .Where(q => !q.IsCompleted && q.DueDate <= DateTime.Now.AddHours(24))
                .ToList();
                
            if (urgentQuests.Any())
            {
                AnsiConsole.MarkupLine("\n[red]⚠️ URGENT: You have quests due within 24 hours![/]");
                foreach (var quest in urgentQuests.Take(3)) // Show top 3
                {
                    AnsiConsole.MarkupLine($"[red]   - {quest.Title} (Due: {quest.DueDate:MMM dd HH:mm})[/]");
                }
                
                if (urgentQuests.Count > 3)
                {
                    AnsiConsole.MarkupLine($"[red]   ... and {urgentQuests.Count - 3} more[/]");
                }
                
                AnsiConsole.MarkupLine("[yellow]Check notifications for details.[/]\n");
            }
        }
    }
    catch (Exception ex)
    {
        // Silent fail for background checks
        Console.WriteLine($"Background notification check failed: {ex.Message}");
    }
}

    
 
}
}
