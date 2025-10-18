// Services/SimpleMusicService.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace QuestGuildTerminal.Services
{
    public class SimpleMusicService : IMusicService
    {
        private Process _musicProcess;
        private bool _isPlaying = false;

        public bool IsPlaying => _isPlaying;
        public float Volume { get; set; } = 0.3f;

        public Task PlayBackgroundMusicAsync(string filePath)
        {
            try
            {
                Stop(); // Stop any existing music

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"❌ Music file not found: {filePath}");
                    Console.WriteLine($"💡 Looking in: {Path.GetDirectoryName(filePath)}");
                    return Task.CompletedTask;
                }

                Console.WriteLine($"🎵 Attempting to play: {filePath}");

                // Use system audio player based on OS
                if (OperatingSystem.IsMacOS())
                {
                    // macOS - use afplay
                    _musicProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "afplay",
                            Arguments = $"\"{filePath}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };
                    
                    _musicProcess.Start();
                    _isPlaying = true;
                    Console.WriteLine("✅ Music started with afplay (macOS)");
                }
                else if (OperatingSystem.IsWindows())
                {
                    // Windows - use Media Player
                    _musicProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "wmplayer",
                        Arguments = $"\"{filePath}\"",
                        UseShellExecute = true
                    });
                    _isPlaying = true;
                    Console.WriteLine("✅ Music started with Windows Media Player");
                }
                else
                {
                    // Linux - try ffplay or mplayer
                    Console.WriteLine("❌ Audio not supported on this platform");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Could not play music: {ex.Message}");
                _isPlaying = false;
            }

            return Task.CompletedTask;
        }

        public void Stop()
        {
            try
            {
                if (_musicProcess != null && !_musicProcess.HasExited)
                {
                    _musicProcess.Kill();
                    _musicProcess.Dispose();
                }
                _musicProcess = null;
                _isPlaying = false;
                Console.WriteLine("🎵 Music stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error stopping music: {ex.Message}");
            }
        }

        public void Pause()
        {
            // Can't easily pause system processes, so we stop instead
            Stop();
            Console.WriteLine("🎵 Music paused");
        }

        public void Resume()
        {
            // Can't resume, need to call PlayBackgroundMusicAsync again
            Console.WriteLine("💡 Use 'Play Music' to restart the music");
        }

        public void SetVolume(float volume)
        {
            Volume = Math.Clamp(volume, 0f, 1f);
            Console.WriteLine($"🔊 Volume set to {Volume * 100}%");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}