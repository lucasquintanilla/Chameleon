using Core.Entities;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Core.Shared
{
    public class FileStoreService
    {
        private IWebHostEnvironment _env;

        private string _dir;

        private static readonly string[] permittedExtensions = new[]
        { 
            ".png", ".jpg", ".jpeg", ".gif", ".webp" 
        };

        private readonly string folderName = "media";

        private int maxFileSize = 10 * 1024 * 1024;

        const string ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";

        public FileStoreService(IWebHostEnvironment env)
        {
            _env = env;
            _dir = env.WebRootPath;
            FFmpeg.SetExecutablesPath(@"C:\FFmpeg");

            string folderPath = Path.Combine(_dir, folderName);
           
            Directory.CreateDirectory(folderPath);
            
        }

        public async Task<bool> IsValidFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                // The extension is invalid ... discontinue processing the file
                return false;
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                // Upload the file if less than X MB
                if (memoryStream.Length > maxFileSize 
                    || memoryStream.Length == 225467 //Gif fuck
                    || memoryStream.Length == 963 //GIF blanco y negro
                    )
                {                    
                    return false;
                }
            }

            return true;
        }

        public async Task<Media> Save(IFormFile file, string hash) 
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            var originalFileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var originalFilename = $"{DateTime.Now:yyyyMMdd}-{hash}{originalFileExtension}";
            var originalFilePath = Path.Combine(_dir, folderName, originalFilename);

            var thumbnailFilename = $"{DateTime.Now:yyyyMMdd}-{hash}.webp";
            var thumbnailFilePath = Path.Combine(_dir, folderName, thumbnailFilename);

            if (IsGif(file))
            {
                //CONVERT AND SAVE GIF TO WEBP
                //ConvertoAndSaveToWebp(file, filePath);                

                await SaveGifThumbnail(file, thumbnailFilePath);                
                await SaveOriginalFormat(file, originalFilePath);
            }
            else
            {
                await SaveImageThumbnail(file, thumbnailFilePath);
                await SaveOriginalFormat(file, originalFilePath);
            }

            return new Media
            {
                Url = $"/{folderName}/{originalFilename}",
                ThumbnailUrl = $"/{folderName}/{thumbnailFilename}",
                MediaType = MediaType.Image
            };
        }

        public async Task<Media> SaveExternal(Voxed.WebApp.Models.UploadData data, string hash)
        {
            if (data.Extension == "ytb")
            {
                return new Media()
                {
                    ID = Guid.NewGuid(),
                    Url = $"https://www.youtube.com/watch?v={data.ExtensionData}",
                    ThumbnailUrl = await GenerateYoutubeThumbnail(data.ExtensionData, hash),
                    MediaType = MediaType.YouTube,
                };

            }

            return null;
        }

        private async Task SaveOriginalFormat(IFormFile file, string filePath)
        {
            using (var fileStream = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write))
            {

                await file.CopyToAsync(fileStream);
            }
        }

        private bool IsGif(IFormFile file)
            => file.ContentType == MediaTypeNames.Image.Gif;

        private async Task<bool> SaveImageThumbnail(IFormFile file, string filePath)
        {
            // Then save in WebP format
            using var webPFileStream = new FileStream(filePath, FileMode.Create);
            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
            {
                imageFactory.Load(file.OpenReadStream())
                            .Format(new WebPFormat())
                            .Quality(50)
                            .Save(webPFileStream);
            }

            return true;
        }

        private async Task<bool> SaveGifThumbnail(IFormFile gifFile, string outputFilePath)
        {
            var tempPath = _CreateTempAndClear();
            var inputFilePath = _SaveFile(gifFile, tempPath);

            var result = await FFmpeg.Conversions.FromSnippet.Snapshot(inputFilePath,
                                               outputFilePath,
                                               TimeSpan.FromSeconds(0))
                                                .Result.Start();

            return true;

            //var command = string.Format($"-i {inputFilePath} -vsync 0 {outputFilePath}");
            //using (var process = Process.Start(ffmpegPath, command))
            //{
            //    process.WaitForExit();
            //    if (process.ExitCode == 0)
            //    {
            //        return true;
            //    }
            //}

            //throw new Exception("Error al guardar thumbnail.");
        }

        public bool ConvertoAndSaveToWebp(IFormFile gifFile, string outputFilePath)
        {
            var tempPath = _CreateTempAndClear();
            var inputFilePath = _SaveFile(gifFile, tempPath);

            //RESOURCES
            //https://medium.com/pinterest-engineering/improving-gif-performance-on-pinterest-8dad74bf92f1
            //ffmpeg -i $gifPath -movflags faststart -pix_fmt yuv420p -vf “scale=trunc(iw/2)*2:trunc(ih/2)*2” -c:v libx264 $videoPath

            //https://web.dev/replace-gifs-with-videos/
            //ffmpeg -i my-animation.gif -c vp9 -b:v 0 -crf 41 my-animation.webm

            //var command = $"-i {inputFilePath} -vcodec libwebp -lossless 0 -qscale 75 -preset default -loop 0 -vf scale=320:-1,fps=15 -an -vsync 0 {outputFilePath}";


            var command = string.Format ($"-i {inputFilePath} -b:v 0 -crf 25 -loop 0 {outputFilePath}");
            using (var process = Process.Start(ffmpegPath, command))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {                    
                    return true;
                }
            }

            return false;
        }

        string _SaveFile(IFormFile gifFile, string tempPath)
        {
            string inputFileName = "input.gif";
            var inputFilePath = Path.Combine(tempPath, inputFileName); // Unique filename
            using (var fileStream = gifFile.OpenReadStream())
            using (var stream = new FileStream(inputFilePath, FileMode.CreateNew))
            {
                fileStream.CopyTo(stream);
            }
            return inputFilePath;
        }

        string _CreateTempAndClear()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "GifConverter");
            try
            {
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);
                foreach (var file in Directory.GetFiles(tempPath))
                    System.IO.File.Delete(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
            return tempPath;
        }

        private readonly HttpClient client = new HttpClient();

        public async Task<string> GenerateYoutubeThumbnail(string id, string hash)
        {
            var response = await client.GetAsync($"https://img.youtube.com/vi/{id}/maxresdefault.jpg");

            if (!response.IsSuccessStatusCode)
            {
                response = await client.GetAsync($"https://img.youtube.com/vi/{id}/hqdefault.jpg");
            }

            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync();

            var thumbnailFilename = $"{DateTime.Now:yyyyMMdd}-{hash}.jpg";
            var thumbnailFilePath = Path.Combine(_dir, folderName, thumbnailFilename);

            using var fileStream = File.Create(thumbnailFilePath);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);

            return $"/{folderName}/" + thumbnailFilename;
        }

    }
}
