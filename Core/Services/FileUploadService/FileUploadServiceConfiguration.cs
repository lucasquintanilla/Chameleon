using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services.FileUploadService
{
    public class FileUploadServiceConfiguration
    {
        private static readonly string[] permittedExtensions = new[]
           {
            ".png", ".jpg", ".jpeg", ".gif", ".webp"
        };

        public IEnumerable<string> PermittedExtensions { get; set; }
        public string MediaFolderName { get; set; }
        public int MaxFileSize { get; set; }
        public string FFmpegPath { get; set; }
        public int ThumbnailQuality { get; set; }
    }
}
