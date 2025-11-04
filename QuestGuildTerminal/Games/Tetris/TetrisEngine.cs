using System;
using System.IO;
using System.Threading.Tasks;
using QuestGuildTerminal.Services;
using Spectre.Console;
using TetrisOOP.Gameplay; // Add this using directive

namespace QuestGuildTerminal.Games.Tetris
{
    public class TetrisEngine : IGameEngine
    {
        public string GameName => "Tetris";
        
        private readonly IMusicService _musicService;
        private readonly string _gameMusicPath;
        
        // Implement the required events
        public event Action<int> OnLevelUp = delegate { };
        public event Action<bool> OnGameComplete = delegate { };
        
        public TetrisEngine(IMusicService musicService = null)
        {
            _musicService = musicService;
            _gameMusicPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "tetris.mp3");
        }
        
        public async Task<GameResult> StartGameAsync(int targetLevel)
        {
            Console.Clear();
            
            // Show game header
            var gameHeader = new FigletText("TETRIS")
                .Color(Color.Red);
            AnsiConsole.Write(gameHeader);
            
            var challengeHeader = new FigletText("CHALLENGE")
                .Color(Color.Yellow);
            AnsiConsole.Write(challengeHeader);

            // Loading animation
            await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask("[green]INITIALIZING GAME MATRIX[/]");
                    
                    while (!ctx.IsFinished)
                    {
                        task.Increment(1.5);
                        await Task.Delay(50);
                    }
                });

            // Game instructions
            var panel = new Panel(
                new Markup($"[bold blue]🎯 MISSION: REACH LEVEL {targetLevel}[/]\n\n" +
                          "[yellow]CONTROLS:[/]\n" +
                          "← → ↓ : Move Block\n" +
                          "↑/X   : Rotate\n" + 
                          "Z     : Rotate CCW\n" +
                          "SPACE : Hard Drop\n" +
                          "Q     : Quit Game"))
            {
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.Blue),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(panel);
            
            // Handle music
            if (_musicService != null && File.Exists(_gameMusicPath))
            {
                AnsiConsole.MarkupLine("[green]🎵 Switching to game music...[/]");
                await _musicService.SwitchToGameMusicAsync(_gameMusicPath);
            }
            
            AnsiConsole.MarkupLine("\n[bold red]PRESS ANY KEY TO START GAME...[/]");
            Console.ReadKey();

            var result = await RunTetrisGame(targetLevel);

            // Restore background music
            if (_musicService != null)
            {
                AnsiConsole.MarkupLine("[green]🎵 Restoring background music...[/]");
                await _musicService.SwitchToBackgroundMusicAsync();
            }

            return result;
        }
        
        private async Task<GameResult> RunTetrisGame(int targetLevel)
        {
            try
            {
                var startTime = DateTime.Now;
                
                // Use your existing Game class with the target level
                var tetrisGame = new Game(targetLevel);
                
                // Run your existing game - this should now work properly
                await Task.Run(() => tetrisGame.Run());
                
                var timePlayed = DateTime.Now - startTime;
                
                // Get results from your existing game
                bool success = tetrisGame.ReachedTargetLevel;
                int finalLevel = tetrisGame.FinalLevel;
                int finalScore = tetrisGame.FinalScore;
                
                // Show results using Spectre.Console
                Console.Clear();
                
                var missionCompleteText = new FigletText("GAME OVER")
                    .Color(Color.Orange1);
                AnsiConsole.Write(missionCompleteText);
                
                var resultPanel = new Panel(
                    new Markup($"[blue]FINAL LEVEL:[/] [bold yellow]{finalLevel}[/]\n" +
                              $"[blue]TARGET LEVEL:[/] [bold white]{targetLevel}[/]\n" +
                              $"[blue]FINAL SCORE:[/] [bold gold1]{finalScore}[/]\n" +
                              $"[blue]TIME PLAYED:[/] [bold cyan]{timePlayed:mm\\:ss}[/]\n" +
                              $"[blue]MISSION STATUS:[/] [bold {(success ? "green" : "red")}]{(success ? "SUCCESS" : "FAILED")}[/]"))
                {
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(success ? Color.Green : Color.Red),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(resultPanel);
                
                if (success)
                {
                    AnsiConsole.MarkupLine("\n[bold green]🎉 CONGRATULATIONS! QUEST COMPLETED! 🎉[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[bold yellow]💪 Keep practicing! Try again![/]");
                }
                
                AnsiConsole.MarkupLine("\n[blue]Press any key to continue...[/]");
                Console.ReadKey();
                
                return new GameResult 
                { 
                    Success = success, 
                    FinalLevel = finalLevel, 
                    Score = finalScore,
                    TimePlayed = timePlayed
                };
            }
            catch (Exception ex)
            {
                Console.Clear();
                
                AnsiConsole.MarkupLine("[bold red]❌ GAME ERROR:[/]");
                AnsiConsole.MarkupLine($"[yellow]{ex.Message}[/]");
                AnsiConsole.MarkupLine($"[grey]Make sure all TetrisOOP dependencies are available[/]");
                
                if (_musicService != null)
                {
                    await _musicService.SwitchToBackgroundMusicAsync();
                }
                
                AnsiConsole.MarkupLine("\n[blue]Press any key to continue...[/]");
                Console.ReadKey();
                
                return new GameResult { 
                    Success = false, 
                    FinalLevel = 1, 
                    Score = 0,
                    TimePlayed = TimeSpan.Zero
                };
            }
        }
    }
}