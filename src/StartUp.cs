namespace YouTubeToMP3
{
    using YoutubeExplode;
    using YoutubeExplode.Converter;
    using System.Windows.Forms;
    using YouTubeToMP3.Core.Contracts;
    using YouTubeToMP3.Core;
    using YouTubeToMP3.Services.Contracts;
    using YouTubeToMP3.Services;

    public class StartUp
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            YoutubeClient youTube = new YoutubeClient();
            IYouTubeService youTubeService = new YouTubeService(youTube);
            IEngine engine = new Engine(youTubeService);
            await engine.RunAsync();
        }
    }
}
