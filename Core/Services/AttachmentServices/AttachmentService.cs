using Core.Entities;
using Core.Extensions;
using Core.Services.AttachmentServices.Models;
using Core.Services.Storage;
using Core.Services.Storage.Models;
using Core.Services.Youtube;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Core.Services.AttachmentServices;

public interface IAttachmentService
{
    Task<Attachment> CreateAttachment(CreateAttachmentRequest request);
}

public class AttachmentService : IAttachmentService
{
    private readonly AttachmentServiceConfiguration _config;
    private readonly IYoutubeService _youtubeService;
    private readonly IStorage _storageService;

    public AttachmentService(
        IYoutubeService youtubeService,
        IOptions<AttachmentServiceConfiguration> options,
        IStorage storageService)
    {
        _config = options.Value;
        _youtubeService = youtubeService;
        _storageService = storageService;
    }

    public async Task<Attachment> CreateAttachment(CreateAttachmentRequest request)
    {
        return request.Extension switch
        {
            CreateAttachmentFileExtension.Youtube => await SaveFromYoutube(request.ExtensionData),
            CreateAttachmentFileExtension.Base64 => await SaveFromBase64(request.ExtensionData),
            CreateAttachmentFileExtension.Gif => await SaveImageFromGif(request.File),
            CreateAttachmentFileExtension.Jpg => await SaveImageFromFile(request.File),
            CreateAttachmentFileExtension.Jpeg => await SaveImageFromFile(request.File),
            CreateAttachmentFileExtension.Png => await SaveImageFromFile(request.File),
            CreateAttachmentFileExtension.WebP => await SaveImageFromFile(request.File),
            _ => throw new NotImplementedException("Invalid file extension"),
        };
    }

    private async Task<Attachment> SaveImageFromFile(IFormFile file)
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

        return new Attachment
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = AttachmentType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }

    private async Task<Attachment> SaveImageFromGif(IFormFile file)
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

        return new Attachment
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = AttachmentType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }

    private async Task<Attachment> SaveFromYoutube(string videoId)
    {
        var thumbnail = new StorageObject()
        {
            Key = Guid.NewGuid() + "_thumb.jpeg",
            Content = await _youtubeService.GetThumbnail(videoId),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(thumbnail);

        return new Attachment
        {
            Url = $"https://www.youtube.com/watch?v={videoId}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = AttachmentType.YouTube,
            ExternalUrl = $"https://www.youtube.com/watch?v={videoId}"
        };
    }

    private async Task<Attachment> SaveFromBase64(string base64)
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

        return new Attachment
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = AttachmentType.Image,
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }
}
