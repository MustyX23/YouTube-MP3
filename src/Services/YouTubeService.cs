using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YouTubeToMP3.Services.Contracts;

namespace YouTubeToMP3.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly YoutubeClient youtube;
        public YouTubeService(YoutubeClient youtube)
        {
            this.youtube = youtube;
        }

        public async Task MP3DownloadAsync(string inputUrl, string outputPath)
        {
            // Get video information (including title)
            var video = await youtube.Videos.GetAsync(inputUrl);
            string videoTitle = video.Title; // Extract the video title

            // Remove any invalid characters from the title for a valid file name
            string sanitizedTitle = string.Join("_", videoTitle.Split(Path.GetInvalidFileNameChars()));

            string fullMp3Path = Path.Combine(outputPath, sanitizedTitle + ".mp3");

            // Dynamically locate ffmpeg.exe based on the current directory
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string ffmpegPath = Path.Combine(currentDirectory, "ffmpeg", "ffmpeg.exe"); // Assuming ffmpeg is in a subfolder named "ffmpeg"

            // Download and convert the video to MP3
            await youtube.Videos.DownloadAsync(inputUrl, fullMp3Path, builder => builder
                .SetFFmpegPath(ffmpegPath) // Use dynamic ffmpeg path here
                .SetFormat("mp3"));

            Console.WriteLine($"MP3 saved to: {fullMp3Path}");
        }

        public bool IsPlaylist(string url)
        {
            return url.Contains("list=");
        }

        public string SelectOutputPath()
        {
            string folderPath = string.Empty;

            Thread thread = new Thread(() =>
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select a folder to save your MP3 files.";
                    folderDialog.ShowNewFolderButton = true;

                    DialogResult result = folderDialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                    {
                        folderPath = folderDialog.SelectedPath;
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return folderPath;
        }

        public async Task MP3DownloadPlayListAsync(string inputUrl, string outputPath)
        {
            var downloadTasks = new List<Task>();

            await foreach (var video in youtube.Playlists.GetVideosAsync(inputUrl))
            {
                downloadTasks.Add(MP3DownloadAsync(video.Url, outputPath));
            }

            await Task.WhenAll(downloadTasks);
        }

        public async Task MP4DownloadAsync(string inputUrl, string outputPath)
        {
            var video = GetVideoAsync(inputUrl);

            string videoTitle = video.Result.Title;

            // Remove any invalid characters from the title for a valid file name
            string sanitizedTitle = string.Join("_", videoTitle.Split(Path.GetInvalidFileNameChars()));

            string fullMp4Path = Path.Combine(outputPath, sanitizedTitle + ".mp4");

            // Dynamically locate ffmpeg.exe based on the current directory
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string ffmpegPath = Path.Combine(currentDirectory, "ffmpeg", "ffmpeg.exe"); // Assuming ffmpeg is in a subfolder named "ffmpeg"

            // Download and convert the video to MP3
            await youtube.Videos.DownloadAsync(inputUrl, fullMp4Path, builder => builder
                .SetFFmpegPath(ffmpegPath) // Use dynamic ffmpeg path here
                .SetFormat("mp4"));

            //REFACTORY TODO:

            //Video GetVideoInfo(string inputUrl); --Done
            //string SanitizeVideoTitle(string videoTitle);
            //string CombinePath(outputPath, sanitizedTitle, format);
            //DownloadAsync should pass as a parameter the file format.

            Console.WriteLine($"MP4 saved to: {fullMp4Path}");
        }
        private async Task<Video> GetVideoAsync(string inputUrl)
        {
            var video = await youtube.Videos.GetAsync(inputUrl);
            return video;
        }
    }
}
