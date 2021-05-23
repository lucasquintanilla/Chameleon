using System.Collections.Generic;

namespace Core.Services.FileUploadService
{
    public class FileUploadServiceConfiguration
    {
        public IEnumerable<string> PermittedExtensions { get; set; }
        public string MediaFolderName { get; set; }
        public int MaxFileSize { get; set; }
        public string FFmpegPath { get; set; }
        public int ThumbnailQuality { get; set; }
        public bool UseImxto { get; set; }
    }
}
