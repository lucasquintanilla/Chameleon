﻿namespace Core.Entities
{
    public enum AttachmentType { Image, Video, YouTube, Gif }

    public class Attachment : Entity
    {
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public AttachmentType Type { get; set; }
    }
}