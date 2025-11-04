using System;
using System.Diagnostics;
using System.Threading;
using TetrisOOP.Rendering;

namespace TetrisOOP.Gameplay
{
    public class Game
    {
        private readonly Board board;
        private Tetromino current;
        private Tetromino next;
        private bool gameOver = false;
        private int score = 0;
        private int level = 1;
        private int lines = 0;
        private int baseIntervalMs = 700;

        // Quest integration properties
        public bool IsGameOver => gameOver;
        public int FinalScore => score;
        public int FinalLevel => level;
        public int FinalLines => lines;
        public bool ReachedTargetLevel { get; private set; } = false;
        public int TargetLevel { get; set; } = 3;

        public Game(int targetLevel) : this()
        {
            TargetLevel = targetLevel;
        }

        public Game()
        {
            board = new Board(10, 20);
            current = PieceFactory.RandomPiece();
            next = PieceFactory.RandomPiece();
            PlaceNewCurrent();
        }

        private void PlaceNewCurrent()
        {
            current = next;
            current.X = (board.Width - 4) / 2;
            current.Y = 0;
            next = PieceFactory.RandomPiece();
            if (!board.CanPlace(current, current.X, current.Y)) gameOver = true;
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long lastTick = sw.ElapsedMilliseconds;

            // Display quest information
            Console.Clear();
            Console.WriteLine("🎮 TETRIS QUEST CHALLENGE");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine($"🎯 TARGET: Reach Level {TargetLevel}");
            Console.WriteLine("📋 CONTROLS:");
            Console.WriteLine("   ← → ↓ : Move piece");
            Console.WriteLine("   ↑ / X : Rotate clockwise");
            Console.WriteLine("   Z     : Rotate counter-clockwise");
            Console.WriteLine("   Space : Hard drop");
            Console.WriteLine("   Q     : Quit game");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("💡 The game will automatically end when you reach the target level!");
            Console.WriteLine("Press any key to begin your quest...");
            Console.ReadKey();

            while (!gameOver)
            {
                if (Console.KeyAvailable) HandleKey(Console.ReadKey(true).Key);
                long now = sw.ElapsedMilliseconds;
                int interval = Math.Max(100, baseIntervalMs - (level - 1) * 10);

                if (now - lastTick >= interval)
                {
                    if (!TryMove(0, 1))
                    {
                        board.Place(current, current.X, current.Y);
                        int cleared = board.ClearFullLines();
                        if (cleared > 0)
                        {
                            lines += cleared;
                            score += cleared * 100 * level;
                            
                            // Track level progression
                            int newLevel = 1 + lines / 10;
                            if (newLevel > level)
                            {
                                level = newLevel;
                                
                                // NEW: Auto-quit when target level is reached
                                if (level >= TargetLevel && !ReachedTargetLevel)
                                {
                                    ReachedTargetLevel = true;
                                    gameOver = true; // Automatically end the game
                                    break; // Exit the game loop immediately
                                }
                            }
                        }
                        PlaceNewCurrent();
                    }
                    lastTick = now;
                }

                Renderer.DrawBoard(board, current, score, level, TargetLevel);
                Thread.Sleep(15);
            }

            // Enhanced game over screen
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("              GAME OVER                ");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine($"🏆 Final Score: {score}");
            Console.WriteLine($"📊 Lines Cleared: {lines}");
            Console.WriteLine($"🎯 Level Reached: {level}");
            Console.WriteLine($"🎮 Target Level: {TargetLevel}");
            
            if (ReachedTargetLevel)
            {
                Console.WriteLine("🎉 SUCCESS: Quest target achieved! 🎉");
                Console.WriteLine("✅ The game automatically ended as you reached the target!");
            }
            else if (level < TargetLevel)
            {
                Console.WriteLine("❌ Failed to reach target level");
                Console.WriteLine("💡 Try again to complete your quest!");
            }
            else
            {
                Console.WriteLine("🏁 Game completed!");
            }
            
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("Press any key to return to quest menu...");
            Console.ReadKey();
        }

        private void HandleKey(ConsoleKey k)
        {
            switch (k)
            {
                case ConsoleKey.LeftArrow: TryMove(-1, 0); break;
                case ConsoleKey.RightArrow: TryMove(1, 0); break;
                case ConsoleKey.DownArrow: TryMove(0, 1); break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.X: TryRotate(true); break;
                case ConsoleKey.Z: TryRotate(false); break;
                case ConsoleKey.Spacebar: while (TryMove(0, 1)) { } break;
                case ConsoleKey.Q: 
                    gameOver = true; 
                    Console.WriteLine("\n🚪 Game quit by player");
                    break;
            }
        }

        private bool TryMove(int dx, int dy)
        {
            int newX = current.X + dx, newY = current.Y + dy;
            if (board.CanPlace(current, newX, newY))
            {
                current.X = newX; current.Y = newY; return true;
            }
            return false;
        }

        private void TryRotate(bool cw)
        {
            if (cw) current.RotateCW(); else current.RotateCCW();
            if (!board.CanPlace(current, current.X, current.Y))
            {
                if (board.CanPlace(current, current.X - 1, current.Y)) current.X -= 1;
                else if (board.CanPlace(current, current.X + 1, current.Y)) current.X += 1;
                else { if (cw) current.RotateCCW(); else current.RotateCW(); }
            }
        }
    }
}