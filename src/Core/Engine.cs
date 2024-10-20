namespace YouTubeToMP3.Core
{
    using YouTubeToMP3.Core.Contracts;
    using YouTubeToMP3.Services.Contracts;

    public class Engine : IEngine
    {
        private readonly IYouTubeService youTubeService;
        public Engine(IYouTubeService youTubeService)
        {
            this.youTubeService = youTubeService;
        }
        public async Task RunAsync()
        {
            // URL of the YouTube video to download
            Console.WriteLine("Enter the URL video here: ");
            string inputUrl = Console.ReadLine();

            Console.WriteLine("Where do you want to download it to? (Enter path!)");
            // Path to save the MP3 file with the video title
            string outPutPath = youTubeService.SelectOutputPath();

            if (youTubeService.IsPlaylist(inputUrl))
            {
                while (inputUrl != null)
                {
                    await youTubeService.MP3DownloadPlayListAsync(inputUrl, outPutPath);
                    Console.WriteLine("Download finished! Another Playlist?");
                    inputUrl = Console.ReadLine();
                }                
            }
            else
            {
                while (inputUrl != null)
                {
                    await youTubeService.MP3DownloadAsync(inputUrl, outPutPath);
                    Console.WriteLine("Download finished! Another Song?");
                    inputUrl = Console.ReadLine();
                }
            }
        }
    }
}
