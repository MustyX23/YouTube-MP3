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

        // Function to download video and convert to MP3 using YouTube video title as filename
        static async Task DownloadAndConvertToMp3(string videoUrl, string mp3Path)
        {
            var youtube = new YoutubeClient();

            // Get video information (including title)
            var video = await youtube.Videos.GetAsync(videoUrl);
            string videoTitle = video.Title; // Extract the video title

            // Remove any invalid characters from the title for a valid file name
            string sanitizedTitle = string.Join("_", videoTitle.Split(Path.GetInvalidFileNameChars()));

            string fullMp3Path = Path.Combine(mp3Path, sanitizedTitle + ".mp3");

            // Dynamically locate ffmpeg.exe based on the current directory
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string ffmpegPath = Path.Combine(currentDirectory, "ffmpeg", "ffmpeg.exe"); // Assuming ffmpeg is in a subfolder named "ffmpeg"

            // Download and convert the video to MP3
            await youtube.Videos.DownloadAsync(videoUrl, fullMp3Path, builder => builder
                .SetFFmpegPath(ffmpegPath) // Use dynamic ffmpeg path here
                .SetFormat("mp3"));

            Console.WriteLine($"MP3 saved to: {fullMp3Path}");
        }
        static bool IsPlaylistUrl(string url)
        {
            // Check if the URL contains "list=" which is an indicator of a playlist
            return url.Contains("list=");
        }
        static async Task PlaylistDownloadAndConvertToMp3(string playListUrl, string mp3Path)
        {
            var youtube = new YoutubeClient();

            var playlist = youtube.Playlists.GetVideosAsync(playListUrl);

            await foreach (var video in playlist)
            {
                string videoTitle = video.Title;

                string sanitizedTitle = string.Join("_", videoTitle.Split(Path.GetInvalidFileNameChars()));

                string fullMp3Path = Path.Combine(mp3Path, sanitizedTitle + ".mp3");

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string ffmpegPath = Path.Combine(currentDirectory, "ffmpeg", "ffmpeg.exe"); // Assuming ffmpeg is in a subfolder named "ffmpeg"

                await youtube.Videos.DownloadAsync(video.Url, fullMp3Path, builder => builder
                    .SetFFmpegPath(ffmpegPath) // Use dynamic ffmpeg path here
                    .SetFormat("mp3"));

                Console.WriteLine($"MP3 saved to: {fullMp3Path}");
            }
            
        }
        static string SelectDownloadFolder()
        {
            string folder = null;
            Thread thread = new Thread(() =>
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select a folder to save your MP3 files.";
                    folderDialog.ShowNewFolderButton = true;

                    DialogResult result = folderDialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                    {
                        folder = folderDialog.SelectedPath;
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return folder;
        }
    }
}
