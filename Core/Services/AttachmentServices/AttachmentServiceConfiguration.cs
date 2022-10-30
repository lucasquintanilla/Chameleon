﻿using System.Collections.Generic;

namespace Core.Services.AttachmentServices
{
    public class AttachmentServiceConfiguration
    {
        public const string SectionName = "FileUploadService";

        public IEnumerable<string> PermittedExtensions { get; set; }
        public string MediaFolderName { get; set; }
        public int MaxFileSize { get; set; }
        public string FFmpegPath { get; set; }
        public int ThumbnailQuality { get; set; }
        public bool UseImxto { get; set; }
        public string WebRootPath { get; set; }
    }
}