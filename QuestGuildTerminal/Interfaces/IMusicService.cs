// Interfaces/IMusicService.cs
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface IMusicService : IDisposable
    {
        bool IsPlaying { get; }
        double Volume { get; set; }
        
        Task PlayBackgroundMusicAsync(string filePath);
        void Stop();
        void Pause();
        void Resume();
        void SetVolume(double volume);
    }
}