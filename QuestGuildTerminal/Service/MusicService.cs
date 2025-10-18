// Services/MusicService.cs
using System;
using System.Threading.Tasks;
using NAudio.Wave;

namespace QuestGuildTerminal.Services
{
    public class MusicService : IMusicService
    {
        private WaveOutEvent _waveOut;
        private AudioFileReader _audioFile;
        private bool _isDisposed = false;
        private float _volume = 0.5f;

        public bool IsPlaying { get; private set; }
        public float Volume 
        { 
            get => _volume;
            set
            {
                _volume = Math.Clamp(value, 0f, 1f);
                if (_audioFile != null)
                {
                    _audioFile.Volume = _volume;
                }
            }
        }

        // Remove async since we're not using await
        public Task PlayBackgroundMusicAsync(string filePath)
        {
            if (_isDisposed) 
                throw new ObjectDisposedException(nameof(MusicService));
            
            try
            {
                Stop(); // Stop any current music
                
                _audioFile = new AudioFileReader(filePath);
                _audioFile.Volume = _volume;
                
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_audioFile);
                
                // Set up looping
                _waveOut.PlaybackStopped += (sender, e) =>
                {
                    if (!_isDisposed && _audioFile != null)
                    {
                        _audioFile.Position = 0;
                        _waveOut.Play();
                    }
                };
                
                _waveOut.Play();
                IsPlaying = true;
                
                Console.WriteLine("🎵 Background music started!");
                
                return Task.CompletedTask; // Return completed task
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Could not play background music: {ex.Message}");
                return Task.FromException(ex); // Return faulted task
            }
        }

        public void Stop()
        {
            _waveOut?.Stop();
            IsPlaying = false;
        }

        public void Pause()
        {
            _waveOut?.Pause();
            IsPlaying = false;
        }

        public void Resume()
        {
            _waveOut?.Play();
            IsPlaying = true;
        }

        public void SetVolume(float volume)
        {
            Volume = volume;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _waveOut?.Stop();
                _waveOut?.Dispose();
                _audioFile?.Dispose();
                _isDisposed = true;
                IsPlaying = false;
            }
        }
    }
}