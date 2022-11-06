﻿using Core.Entities;
using Core.Extensions;
using Core.Services.Storage;
using Core.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Core.Services.AttachmentServices
{
    public interface IAttachmentService
    {
        Task<Attachment> ProcessAttachment(VoxedAttachment uploadData, IFormFile file);
    }

    public class AttachmentService : IAttachmentService
    {
        private readonly AttachmentServiceConfiguration _config;
        private readonly YoutubeService _youtubeService;
        private readonly IStorageService _storageService;

        public AttachmentService(
            YoutubeService youtubeService,
            IOptions<AttachmentServiceConfiguration> options,
            IStorageService storageService)
        {
            _config = options.Value;
            _youtubeService = youtubeService;
            Initialize();
            _storageService = storageService;
        }

        public async Task<Attachment> ProcessAttachment(VoxedAttachment voxedAttachment, IFormFile file)
        {
            if (voxedAttachment == null && file == null) return null;

            if (voxedAttachment.HasData())
            {
                return voxedAttachment.Extension switch
                {
                    VoxedAttachmentExtension.Youtube => await SaveFromYoutube(voxedAttachment.ExtensionData),
                    VoxedAttachmentExtension.Base64 => SaveFromBase64(voxedAttachment.ExtensionData),
                    _ => throw new NotImplementedException("Invalid file extension"),
                };
            }

            if (file == null)
            {
                return null;
            }


            return await SaveFromFile(file);
        }

        private void Initialize()
        {
            FFmpeg.SetExecutablesPath(Path.Combine(_config.WebRootPath, _config.FFmpegPath));

            Directory.CreateDirectory(Path.Combine(_config.WebRootPath, _config.MediaFolderName));
        }

        private void ValidateFile(IFormFile file)
        {
            var fileExtension = file.GetFileExtension();

            if (!_config.PermittedExtensions.Contains(fileExtension))
            {
                throw new Exception("Formato de archivo no valido.");
            }
        }

        private async Task<Attachment> SaveFromFile(IFormFile file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            var originalFilename = GetNormalizedFileName(".jpg");
            var originalFilePath = Path.Combine(_config.WebRootPath, _config.MediaFolderName, originalFilename);

            var thumbnailFilename = GetNormalizedFileName(".webp");
            var thumbnailFilePath = Path.Combine(_config.WebRootPath, _config.MediaFolderName, thumbnailFilename);

            if (file.IsGif())
            {
                //CONVERT AND SAVE GIF TO WEBP
                //ConvertoAndSaveToWebp(file, filePath);                

                await SaveGifThumbnail(file, thumbnailFilePath);
                await file.SaveAsync(originalFilePath);

                return GetLocalMediaResponse(originalFilename, thumbnailFilename, AttachmentType.Gif);
            }

            file.SaveThumbnailAsWebP(thumbnailFilePath);
            file.SaveCompressedAsJpeg(originalFilePath);

            //test
            await _storageService.Upload(file.OpenReadStream());
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
            var originalFilename = GetNormalizedFileName(".jpg");
            var originalFilePath = GetFilePath(originalFilename);

            var thumbnailFilename = GetNormalizedFileName(".webp");
            var thumbnailFilePath = GetFilePath(thumbnailFilename);

            base64.SaveAsJpegFromBase64(originalFilePath);
            base64.SaveThumbnailAsWebPFromBase64(thumbnailFilePath);

            return GetLocalMediaResponse(originalFilename, thumbnailFilename, AttachmentType.Image);
        }

        private string GetFilePath(string filename)
        {
            return Path.Combine(_config.WebRootPath, _config.MediaFolderName, filename);
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
            var thumbnailFilePath = Path.Combine(_config.WebRootPath, _config.MediaFolderName, thumbnailFilename);

            stream.SaveThumbnailAsJpeg(thumbnailFilePath);

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
