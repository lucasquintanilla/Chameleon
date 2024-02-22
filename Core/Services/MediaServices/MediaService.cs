using Core.Entities;
using Core.Extensions;
using Core.Services.Image;
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
    string Location { get; }
}

public class MediaService : IMediaService
{
    private readonly MediaServiceOptions _config;
    private readonly IYoutubeService _youtubeService;
    private readonly IStorage<StorageObject> _storageService;
    private readonly IImageService _imageService;

    public string Location => _config.BaseDirectory;    

    public MediaService(
        IYoutubeService youtubeService,
        IOptions<MediaServiceOptions> options,
        IStorage<StorageObject> storageService,
        IImageService imageService)
    {
        _config = options.Value;
        _youtubeService = youtubeService;
        _storageService = storageService;
        _imageService = imageService;
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
            CreateMediaFileExtension.WebM => await SaveVideoFromFile(request.File),
            CreateMediaFileExtension.Mp4 => await SaveVideoFromFile(request.File),
            _ => throw new NotImplementedException("Invalid file extension"),
        };
    }

    private async Task<Media> SaveImageFromFile(IFormFile file)
    {
        if (file == null) return null;

        var original = new StorageObject()
        {
            Key = Guid.NewGuid() + file.GetFileExtension(),
            Content = file.OpenReadStream(),
            ContentType = file.ContentType
        };
        await _storageService.Save(original);

        return new Media
        {
            Url = $"/{_config.BaseDirectory}/{original.Key}",
            Type = MediaType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }

    private async Task<Media> SaveVideoFromFile(IFormFile file)
    {
        if (file == null) return null;

        var original = new StorageObject()
        {
            Key = Guid.NewGuid() + file.GetFileExtension(),
            Content = file.OpenReadStream(),
            ContentType = file.ContentType
        };
        await _storageService.Save(original);

        return new Media
        {
            Url = $"/{_config.BaseDirectory}/{original.Key}",
            Type = MediaType.Video,
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

        return new Media
        {
            Url = $"/{_config.BaseDirectory}/{original.Key}",
            Type = MediaType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }

    private async Task<Media> SaveFromYoutube(string videoId)
    {
        var thumbnail = new StorageObject()
        {
            Key = Guid.NewGuid() + ".jpeg",
            Content = await _youtubeService.GetThumbnail(videoId),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(thumbnail);

        return new Media
        {
            Url = $"/{_config.BaseDirectory}/{thumbnail.Key}",
            Type = MediaType.YouTube,
            Key = thumbnail.Key,
            ExternalUrl = videoId
        };
    }

    private async Task<Media> SaveFromBase64(string base64)
    {
        var original = new StorageObject()
        {
            Key = Guid.NewGuid() + ".jpeg",
            Content = _imageService.GetStreamFromBase64(base64),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(original);

        return new Media
        {
            Url = $"/{_config.BaseDirectory}/{original.Key}",
            Type = MediaType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }
}
