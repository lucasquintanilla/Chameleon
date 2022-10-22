using Core.Entities;
using System;

namespace Core.Services.AttachmentProcessors
{

    internal class YoutubeAttachmentProcessor : IAttachmentProcessor
    {
        //private readonly YoutubeService _youtubeService;

        //public YoutubeAttachment(YoutubeService youtubeService)
        //{
        //    _youtubeService = youtubeService;
        //}

        public Attachment Process()
        {
            throw new NotImplementedException();
            //var thumbnailFilename = GenerateYoutubeThumbnail(videoId);

            //return new Attachment()
            //{
            //    Url = $"https://www.youtube.com/watch?v={videoId}",
            //    ThumbnailUrl = $"/{_config.MediaFolderName}/" + thumbnailFilename,
            //    Type = AttachmentType.YouTube,
            //};
        }

        //private string GenerateYoutubeThumbnail(string videoId)
        //{
        //    var stream = _youtubeService.GetYoutubeThumbnailStream(videoId).GetAwaiter();

        //    var thumbnailFilename = GetNormalizedFileName(".jpg");
        //    var thumbnailFilePath = Path.Combine(_env.WebRootPath, _config.MediaFolderName, thumbnailFilename);

        //    stream.SaveJPEGThumbnail(thumbnailFilePath);

        //    return thumbnailFilename;
    }

    internal interface IAttachmentProcessor
    {
        Attachment Process();
    }
}
