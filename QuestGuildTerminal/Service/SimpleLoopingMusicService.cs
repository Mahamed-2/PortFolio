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

        public bool IsPlaying => _isPlaying;
        public float Volume { get; set; } = 0.3f;

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
                        Console.WriteLine("🔁 Restarting music...");
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
                
                // Kill any running afplay processes (macOS)
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

        public void SetVolume(float volume)
        {
            Volume = Math.Clamp(volume, 0f, 1f);
            Console.WriteLine($"🔊 Volume set to {Volume * 100}%");
        }

        public void Dispose()
        {
            Stop();
            _cancellationTokenSource?.Dispose();
        }
    }
}