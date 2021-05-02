using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Media
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public MediaType MediaType { get; set; }
    }

    public enum MediaType { Image, Video, YouTube, Gif }
}
