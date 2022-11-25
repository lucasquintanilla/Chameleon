using Core.Entities;
using Core.Extensions;
using Core.Services.MediaServices.Models;
using Core.Services.Storage;
using Core.Services.Storage.Models;
using Core.Services.Youtube;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Core.Services.MediaServices;

public interface IMediaService
{
    Task<Media> CreateMedia(CreateMediaRequest request);
}

public class MediaService : IMediaService
{
    private readonly MediaServiceOptions _config;
    private readonly IYoutubeService _youtubeService;
    private readonly IStorage _storageService;

    public MediaService(
        IYoutubeService youtubeService,
        IOptions<MediaServiceOptions> options,
        IStorage storageService)
    {
        _config = options.Value;
        _youtubeService = youtubeService;
        _storageService = storageService;
    }

    public async Task<Media> CreateMedia(CreateMediaRequest request)
    {
        return request.Extension switch
        {
            CreateMediaFileExtension.Youtube => await SaveFromYoutube(request.ExtensionData),
            CreateMediaFileExtension.Base64 => await SaveFromBase64(request.ExtensionData),
            CreateMediaFileExtension.Gif => await SaveImageFromGif(request.File),
            CreateMediaFileExtension.Jpg => await SaveImageFromFile(request.File),
            CreateMediaFileExtension.Jpeg => await SaveImageFromFile(request.File),
            CreateMediaFileExtension.Png => await SaveImageFromFile(request.File),
            CreateMediaFileExtension.WebP => await SaveImageFromFile(request.File),
            _ => throw new NotImplementedException("Invalid file extension"),
        };
    }

    private async Task<Media> SaveImageFromFile(IFormFile file)
    {
        if (file == null) return null;

        var original = new StorageObject()
        {
            Key = Guid.NewGuid() + file.GetFileExtension(),
            Content = file.OpenReadStream().Compress(),
            ContentType = file.ContentType
        };
        await _storageService.Save(original);

        var thumbnail = new StorageObject()
        {
            Key = Guid.NewGuid() + "_thumb.jpeg",
            Content = file.OpenReadStream().GenerateThumbnail(),
            ContentType = file.ContentType
        };
        await _storageService.Save(thumbnail);

        return new Media
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = MediaType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }

    private async Task<Media> SaveImageFromGif(IFormFile file)
    {
        if (file == null) return null;

        var original = new StorageObject()
        {
            Key = Guid.NewGuid() + file.GetFileExtension(),
            Content = file.OpenReadStream(),
            ContentType = file.ContentType
        };
        await _storageService.Save(original);

        var thumbnail = new StorageObject()
        {
            Key = Guid.NewGuid() + "_thumb.jpeg",
            Content = file.OpenReadStream().GenerateThumbnail(),
            ContentType = file.ContentType
        };
        await _storageService.Save(thumbnail);

        return new Media
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = MediaType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }

    private async Task<Media> SaveFromYoutube(string videoId)
    {
        var thumbnail = new StorageObject()
        {
            Key = Guid.NewGuid() + "_thumb.jpeg",
            Content = await _youtubeService.GetThumbnail(videoId),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(thumbnail);

        return new Media
        {
            Url = $"https://www.youtube.com/watch?v={videoId}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = MediaType.YouTube,
            ExternalUrl = $"https://www.youtube.com/watch?v={videoId}"
        };
    }

    private async Task<Media> SaveFromBase64(string base64)
    {
        var original = new StorageObject()
        {
            Key = Guid.NewGuid() + ".jpeg",
            Content = base64.GetStreamFromBase64(),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(original);

        var thumbnail = new StorageObject()
        {
            Key = Guid.NewGuid() + "_thumb.jpeg",
            Content = base64.GetStreamFromBase64().GenerateThumbnail(),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(thumbnail);

        return new Media
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = MediaType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }
}
