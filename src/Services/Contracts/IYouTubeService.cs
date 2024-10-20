namespace YouTubeToMP3.Services.Contracts
{
    public interface IYouTubeService
    {
        Task MP3DownloadAsync(string inputUrl, string outputPath);
        Task MP3DownloadPlayListAsync(string inputUrl, string outputPath);
        bool IsPlaylist(string url);
        string SelectOutputPath();
    }
}
