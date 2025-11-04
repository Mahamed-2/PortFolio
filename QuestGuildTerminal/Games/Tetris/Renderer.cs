using System;

namespace TetrisOOP.Rendering
{
    public static class Renderer
    {
        private const char BlockChar = '█';

        // Original method for backward compatibility
        public static void DrawBoard(Gameplay.Board board, Gameplay.Tetromino active, int score, int level)
        {
            DrawBoard(board, active, score, level, 3); // Default target level
        }

        // NEW: Overload with target level for quest integration
        public static void DrawBoard(Gameplay.Board board, Gameplay.Tetromino active, int score, int level, int targetLevel)
        {
            Console.Clear();
            int width = board.Width;
            int height = board.Height;

            // --- Calculate horizontal centering ---
            int totalBoardWidth = width * 2 + 4; // 2 chars per block + borders
            int leftPadding = Math.Max(0, (Console.WindowWidth - totalBoardWidth) / 2);

            // --- Calculate vertical centering ---
            int totalBoardHeight = height + 8; // Increased for quest info
            int topPadding = Math.Max(0, (Console.WindowHeight - totalBoardHeight) / 2);

            // --- Print header with quest info (centered) ---
            Console.SetCursorPosition(leftPadding, topPadding);
            
            // NEW: Quest progress display
            string questStatus = level >= targetLevel ? "🎉 QUEST COMPLETE!" : $"🎯 Quest: Level {level}/{targetLevel}";
            Console.WriteLine(questStatus);
            
            Console.SetCursorPosition(leftPadding, topPadding + 1);
            Console.WriteLine($"🏆 Score: {score}  📊 Level: {level}");
            
            Console.SetCursorPosition(leftPadding, topPadding + 2);
            Console.WriteLine(new string('-', width * 2 + 6));

            // Copy board grid and overlay active piece
            var grid = board.GetGridCopy();
            if (active != null)
            {
                for (int r = 0; r < 4; r++)
                    for (int c = 0; c < 4; c++)
                        if (active.GetCell(r, c))
                        {
                            int x = active.X + c, y = active.Y + r;
                            if (y >= 0 && y < height && x >= 0 && x < width)
                                grid[y, x] = (int)active.Color;
                        }
            }

            // --- Print board rows (centered) ---
            for (int r = 0; r < height; r++)
            {
                Console.SetCursorPosition(leftPadding, topPadding + r + 3);
                Console.Write("| ");
                for (int c = 0; c < width; c++)
                {
                    if (grid[r, c] == -1) Console.Write("  ");
                    else
                    {
                        var orig = Console.ForegroundColor;
                        Console.ForegroundColor = (ConsoleColor)grid[r, c];
                        Console.Write(BlockChar); Console.Write(BlockChar);
                        Console.ForegroundColor = orig;
                    }
                }
                Console.WriteLine(" |");
            }

            // --- Footer ---
            Console.SetCursorPosition(leftPadding, topPadding + height + 3);
            Console.WriteLine(new string('-', width * 2 + 6));

            // NEW: Enhanced footer with quest progress bar
            Console.SetCursorPosition(leftPadding, topPadding + height + 4);
            
            // Progress bar visualization
            int progressWidth = 20;
            int progress = (int)Math.Min(level * 100.0 / targetLevel, 100);
            int filled = (progress * progressWidth) / 100;
            
            Console.Write("Progress: [");
            Console.Write(new string('█', filled));
            Console.Write(new string('░', progressWidth - filled));
            Console.WriteLine($"] {progress}%");

            Console.SetCursorPosition(leftPadding, topPadding + height + 5);
            Console.WriteLine("← → move | ↓ drop | ↑/X rotate | Space hard drop | Q quit");

            // NEW: Display motivational message when close to target
            if (level == targetLevel - 1)
            {
                Console.SetCursorPosition(leftPadding, topPadding + height + 6);
                Console.WriteLine("💪 Almost there! One more level to complete your quest!");
            }
            else if (level >= targetLevel)
            {
                Console.SetCursorPosition(leftPadding, topPadding + height + 6);
                Console.WriteLine("🎉 Quest Target Achieved! Keep playing or press Q to finish.");
            }
        }
    }
}