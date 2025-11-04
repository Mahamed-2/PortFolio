// Services/SimpleLoopingMusicService.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace QuestGuildTerminal.Services
{
    public class SimpleLoopingMusicService : IMusicService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isPlaying = false;
        private string _currentMusicPath;
        
        // NEW: Music switching properties
        private string _backgroundMusicPath;
        private string _gameMusicPath;
        private bool _isGameMusicPlaying = false;
        public bool IsPlaying => _isPlaying;
        public double Volume { get; set; } = 1.0; // Use double to match IMusicService.Volume

        public Task PlayBackgroundMusicAsync(string filePath)
        {
            try
            {
                Stop();

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"❌ Music file not found: {filePath}");
                    return Task.CompletedTask;
                }

                _currentMusicPath = filePath;
                
                // NEW: Store as background music if not currently in game mode
                if (!_isGameMusicPlaying)
                {
                    _backgroundMusicPath = filePath;
                }
                
                _cancellationTokenSource = new CancellationTokenSource();
                _isPlaying = true;

                // Start the looping process
                Task.Run(() => MusicLoopAsync(), _cancellationTokenSource.Token);

                Console.WriteLine($"🎵 Music started: {Path.GetFileName(filePath)}");
                Console.WriteLine("🔁 Auto-looping enabled");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Could not start music: {ex.Message}");
                _isPlaying = false;
                return Task.CompletedTask;
            }
        }

        // NEW: Play game-specific music
        public async Task PlayGameMusicAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                Stop(); // Stop current music
                _gameMusicPath = filePath;
                _currentMusicPath = filePath;
                _isGameMusicPlaying = true;
                
                Console.WriteLine($"🎮 Switching to game music: {Path.GetFileName(filePath)}");
                await PlayBackgroundMusicAsync(filePath);
            }
            else
            {
                Console.WriteLine($"❌ Game music file not found: {filePath}");
            }
        }

        // NEW: Switch back to background music
        public async Task SwitchToBackgroundMusicAsync()
        {
            if (!string.IsNullOrEmpty(_backgroundMusicPath) && File.Exists(_backgroundMusicPath))
            {
                Stop(); // Stop current music
                _currentMusicPath = _backgroundMusicPath;
                _isGameMusicPlaying = false;
                
                Console.WriteLine("🎵 Switching back to background music");
                await PlayBackgroundMusicAsync(_backgroundMusicPath);
            }
            else
            {
                Console.WriteLine("⚠️ No background music available to switch back to");
            }
        }

        // NEW: Switch to game music
        public async Task SwitchToGameMusicAsync(string gameMusicPath)
        {
            if (File.Exists(gameMusicPath))
            {
                // Store current background music path if not already stored
                if (string.IsNullOrEmpty(_backgroundMusicPath) && !_isGameMusicPlaying)
                {
                    _backgroundMusicPath = _currentMusicPath;
                }

                await PlayGameMusicAsync(gameMusicPath);
            }
            else
            {
                Console.WriteLine($"❌ Game music file not found: {gameMusicPath}");
                Console.WriteLine("💡 Continuing with current music");
            }
        }

        private async Task MusicLoopAsync()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested && _isPlaying)
            {
                try
                {
                    await PlaySingleInstance();
                    
                    // If we get here, the song finished - wait a moment then restart
                    if (_isPlaying && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        await Task.Delay(500); // Brief pause between loops
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Music playback error: {ex.Message}");
                    await Task.Delay(2000); // Wait before retrying
                }
            }
        }

        private async Task PlaySingleInstance()
        {
            Process process = null;
            
            try
            {
                if (OperatingSystem.IsMacOS())
                {
                    process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "afplay",
                        Arguments = $"\"{_currentMusicPath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }
                else if (OperatingSystem.IsWindows())
                {
                    process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c start /min wmplayer \"{_currentMusicPath}\"",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsLinux())
                {
                    process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "ffplay",
                        Arguments = $"-nodisp -autoexit \"{_currentMusicPath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }

                if (process != null)
                {
                    // Wait for the process to complete (one playthrough)
                    await process.WaitForExitAsync();
                }
                else
                {
                    // If process couldn't start, simulate song length
                    await Task.Delay(10000);
                }
            }
            finally
            {
                process?.Dispose();
            }
        }

        public void Stop()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _isPlaying = false;
                
                // Kill any running music processes
                if (OperatingSystem.IsMacOS())
                {
                    var killProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "pkill",
                        Arguments = "afplay",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    killProcess?.WaitForExit();
                }
                else if (OperatingSystem.IsWindows())
                {
                    // Kill Windows Media Player processes
                    var killProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = "/f /im wmplayer.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    killProcess?.WaitForExit();
                }
                else if (OperatingSystem.IsLinux())
                {
                    var killProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "pkill",
                        Arguments = "ffplay",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    killProcess?.WaitForExit();
                }
                
                Console.WriteLine("🎵 Music stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error stopping music: {ex.Message}");
            }
        }

        public void Pause()
        {
            Stop();
            Console.WriteLine("🎵 Music paused");
        }

        public void Resume()
        {
            if (!_isPlaying && !string.IsNullOrEmpty(_currentMusicPath))
            {
                PlayBackgroundMusicAsync(_currentMusicPath).Wait();
            }
        }

        // NEW: Set volume (double) to match IMusicService interface
        public void SetVolume(double volume)
        {
            Volume = Math.Clamp(volume, 0.0, 1.0);
            Console.WriteLine($"🔊 Volume set to {Volume * 100}%");
            
            // Note: Volume control for external players is limited
            // This mainly tracks the volume level for UI purposes
        }
        
        // Backward-compatible float overload
        public void SetVolume(float volume)
        {
            SetVolume((double)volume);
        }

        public void Dispose()
        {
            Stop();
            _cancellationTokenSource?.Dispose();
        }
        
        // NEW: Get current music type for debugging
        public string GetCurrentMusicType()
        {
            return _isGameMusicPlaying ? "Game Music" : "Background Music";
        }
        
        // NEW: Get current music file name
        public string GetCurrentMusicFile()
        {
            return Path.GetFileName(_currentMusicPath);
        }
    }
}