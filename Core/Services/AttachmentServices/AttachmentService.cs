using Core.Entities;
using Core.Extensions;
using Core.Services.Storage;
using Core.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Core.Services.AttachmentServices;

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
        _storageService = storageService;
    }

    public async Task<Attachment> ProcessAttachment(VoxedAttachment voxedAttachment, IFormFile file)
    {
        if (voxedAttachment == null && file == null) return null;

        return voxedAttachment.Extension switch
        {
            VoxedAttachmentExtension.Youtube => await SaveFromYoutube(voxedAttachment.ExtensionData),
            VoxedAttachmentExtension.Base64 => await SaveFromBase64(voxedAttachment.ExtensionData),
            VoxedAttachmentExtension.Gif => await SaveImageFromFile(file),
            VoxedAttachmentExtension.Jpg => await SaveImageFromFile(file),
            VoxedAttachmentExtension.Jpeg => await SaveImageFromFile(file),
            VoxedAttachmentExtension.Png => await SaveImageFromFile(file),
            VoxedAttachmentExtension.WebP => await SaveImageFromFile(file),
            _ => throw new NotImplementedException("Invalid file extension"),
        };
    }

    private async Task<Attachment> SaveImageFromFile(IFormFile file)
    {
        if (file == null) return null;

        var original = new StorageObject()
        {
            Key = Guid.NewGuid() + file.GetFileExtension(),
            Stream = file.OpenReadStream(),
            ContentType = file.ContentType
        };
        await _storageService.Save(original);

        var thumbnail = new StorageObject()
        {
            Key = "thumbnails/" + Guid.NewGuid() + ".jpeg",
            Stream = file.OpenReadStream().GenerateThumbnail(),
            ContentType = file.ContentType
        };
        await _storageService.Save(thumbnail);

        return new Attachment
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = AttachmentType.Image,
            //new
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }

    private async Task<Attachment> SaveFromYoutube(string videoId)
    {
        var thumbnail = new StorageObject()
        {
            Key = "thumbnails/" + Guid.NewGuid() + ".jpeg",
            Stream = await _youtubeService.GetYoutubeThumbnailStream(videoId),
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
            Stream = base64.GetStreamFromBase64(),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(original);

        var thumbnail = new StorageObject()
        {
            Key = "thumbnails/" + Guid.NewGuid() + ".jpeg",
            Stream = base64.GetStreamFromBase64().GenerateThumbnail(),
            ContentType = "image/jpeg"
        };
        await _storageService.Save(thumbnail);

        return new Attachment
        {
            Url = $"/post-attachments/{original.Key}",
            ThumbnailUrl = $"/post-attachments/{thumbnail.Key}",
            Type = AttachmentType.Image,
            //new
            Key = original.Key,
            ContentType = original.ContentType,
        };
    }
}
