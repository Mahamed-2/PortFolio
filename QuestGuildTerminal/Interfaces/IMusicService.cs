using System.Threading.Tasks;

namespace QuestGuildTerminal.Services  // Use .Services namespace
{
    public interface IMusicService
    {
        bool IsPlaying { get; }
        double Volume { get; set; }
        Task PlayBackgroundMusicAsync(string filePath);
        void Pause();
        void Resume();
        void Stop();
        void SetVolume(double volume);
        void Dispose();
        Task PlayGameMusicAsync(string filePath);
        Task SwitchToBackgroundMusicAsync();
        Task SwitchToGameMusicAsync(string gameMusicPath);
        string GetCurrentMusicType();
        string GetCurrentMusicFile();
    }
}