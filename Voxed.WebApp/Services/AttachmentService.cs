using Core.Entities;
using Core.Services.FileUploadService;
using Core.Services.ImxtoService;
using Core.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Services;
using Xabe.FFmpeg;

namespace Core.Shared
{
    public class AttachmentService
    {
        private readonly IWebHostEnvironment _env;
        private readonly FileUploadServiceConfiguration _config;
        private readonly ImxtoService _imxtoService;
        private readonly CopService _cop;
        private readonly YoutubeService _youtubeService;

        public AttachmentService(
            IWebHostEnvironment env,
            ImxtoService imxtoService,
            YoutubeService youtubeService,
            IOptions<FileUploadServiceConfiguration> options
            )
        {
            _env = env;
            _config = options.Value;
            _imxtoService = imxtoService;
            _cop = new CopService(Path.Combine(_env.WebRootPath, "media", "banned"));
            _youtubeService = youtubeService;
            Initialize();
        }

        public async Task<Attachment> ProcessAttachment(UploadData uploadData, IFormFile file)
        {
            if (uploadData == null && file == null) return null;

            if (uploadData.HasData())
            {
                return uploadData.Extension switch
                {
                    UploadDataExtension.Youtube => await SaveFromYoutube(uploadData.ExtensionData),
                    UploadDataExtension.Base64 => SaveFromBase64(uploadData.ExtensionData),
                    _ => throw new NotImplementedException("Invalid file extension"),
                };
            }

            if (file == null)
            {
                return null;
            }

            ValidateFile(file);

            return await SaveFromFile(file);
        }

        private void Initialize()
        {
            FFmpeg.SetExecutablesPath(Path.Combine(_env.WebRootPath, _config.FFmpegPath));

            Directory.CreateDirectory(Path.Combine(_env.WebRootPath, _config.MediaFolderName));
        }

        private void ValidateFile(IFormFile file)
        {
            var fileExtension = file.GetFileExtension();

            if (!_config.PermittedExtensions.Contains(fileExtension))
            {
                throw new Exception("Formato de archivo no valido.");
            }

            if (_cop.ShouldBeArrested(Image.FromStream(file.OpenReadStream())))
            {
                throw new NotImplementedException("Llamando al 911...");
            }
        }

        private async Task<Attachment> SaveFromFile(IFormFile file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            if (_config.UseImxto)
            {
                var imxtoFile = await _imxtoService.Upload(file.OpenReadStream());

                return new Attachment()
                {
                    Type = AttachmentType.Image,
                    ThumbnailUrl = imxtoFile.ThumbnailUrl,
                    Url = imxtoFile.OriginalUrl
                };
            }

            //var originalFilename = GetNormalizedFileName(hash, file.GetFileExtension());
            var originalFilename = GetNormalizedFileName(".jpg");
            var originalFilePath = Path.Combine(_env.WebRootPath, _config.MediaFolderName, originalFilename);

            var thumbnailFilename = GetNormalizedFileName(".webp");
            var thumbnailFilePath = Path.Combine(_env.WebRootPath, _config.MediaFolderName, thumbnailFilename);

            if (file.IsGif())
            {
                //CONVERT AND SAVE GIF TO WEBP
                //ConvertoAndSaveToWebp(file, filePath);                

                await SaveGifThumbnail(file, thumbnailFilePath);
                await file.SaveAsync(originalFilePath);

                return GetLocalMediaResponse(originalFilename, thumbnailFilename, AttachmentType.Gif);
            }

            file.SaveWEBPThumbnail(thumbnailFilePath);
            file.SaveJPEGCompressed(originalFilePath);

            return GetLocalMediaResponse(originalFilename, thumbnailFilename, AttachmentType.Image);
        }



        private async Task<Attachment> SaveFromYoutube(string videoId)
        {
            var thumbnailFilename = await GenerateYoutubeThumbnail(videoId);

            return new Attachment()
            {
                Url = $"https://www.youtube.com/watch?v={videoId}",
                ThumbnailUrl = $"/{_config.MediaFolderName}/" + thumbnailFilename,
                Type = AttachmentType.YouTube,
            };
        }

        private Attachment SaveFromBase64(string base64)
        {
            var image = base64.GetImageFromBase64();

            if (_cop.ShouldBeArrested(image))
            {
                throw new NotImplementedException("Llamando al 911...");
            }

            //var originalFilename = GetNormalizedFileName(hash, image.GetFileExtension());
            var originalFilename = GetNormalizedFileName(".jpg");
            var originalFilePath = GetFilePath(originalFilename);

            var thumbnailFilename = GetNormalizedFileName(".webp");
            var thumbnailFilePath = GetFilePath(thumbnailFilename);

            image.SaveJPEGCompressed(originalFilePath);

            image.SaveWEBPThumbnail(thumbnailFilePath);

            return GetLocalMediaResponse(originalFilename, thumbnailFilename, AttachmentType.Image);
        }

        private string GetFilePath(string filename)
        {
            return Path.Combine(_env.WebRootPath, _config.MediaFolderName, filename);
        }

        private async Task<bool> SaveGifThumbnail(IFormFile file, string path)
        {
            var tempPath = CreateTempAndClear();
            var inputFilePath = SaveFile(file, tempPath);

            var result = await FFmpeg.Conversions.FromSnippet.Snapshot(inputFilePath,
                                               path,
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
            var tempPath = CreateTempAndClear();
            var inputFilePath = SaveFile(gifFile, tempPath);

            //RESOURCES
            //https://medium.com/pinterest-engineering/improving-gif-performance-on-pinterest-8dad74bf92f1
            //ffmpeg -i $gifPath -movflags faststart -pix_fmt yuv420p -vf “scale=trunc(iw/2)*2:trunc(ih/2)*2” -c:v libx264 $videoPath

            //https://web.dev/replace-gifs-with-videos/
            //ffmpeg -i my-animation.gif -c vp9 -b:v 0 -crf 41 my-animation.webm

            //var command = $"-i {inputFilePath} -vcodec libwebp -lossless 0 -qscale 75 -preset default -loop 0 -vf scale=320:-1,fps=15 -an -vsync 0 {outputFilePath}";


            var command = string.Format($"-i {inputFilePath} -b:v 0 -crf 25 -loop 0 {outputFilePath}");
            using (var process = Process.Start(_config.FFmpegPath, command))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private string SaveFile(IFormFile gifFile, string tempPath)
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

        private string CreateTempAndClear()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "GifConverter");
            try
            {
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);
                foreach (var file in Directory.GetFiles(tempPath))
                    File.Delete(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
            return tempPath;
        }

        private async Task<string> GenerateYoutubeThumbnail(string videoId)
        {
            var stream = await _youtubeService.GetYoutubeThumbnailStream(videoId);

            var thumbnailFilename = GetNormalizedFileName(".jpg");
            var thumbnailFilePath = Path.Combine(_env.WebRootPath, _config.MediaFolderName, thumbnailFilename);

            stream.SaveJPEGThumbnail(thumbnailFilePath);

            return thumbnailFilename;
        }

        private Attachment GetLocalMediaResponse(string originalFilename, string thumbnailFilename, AttachmentType mediaType)
        {
            return new Attachment
            {
                Url = $"/{_config.MediaFolderName}/{originalFilename}",
                ThumbnailUrl = $"/{_config.MediaFolderName}/{thumbnailFilename}",
                Type = mediaType
            };
        }

        private string GetNormalizedFileName(string fileExtension)
        {
            return $"{DateTime.Now:yyyyMMdd}-{Guid.NewGuid()}{fileExtension}";
        }
    }
}
