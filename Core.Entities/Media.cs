using System;

namespace Core.Entities
{
    public enum MediaType { Image, Video, YouTube, Gif }

    public class Media : MutableEntity<Guid>
    {
        public Media()
        {
            Id = Guid.NewGuid();
        }

        public string Url { get; set; } //Eliminar
        public MediaType Type { get; set; }
        public string Key { get; set; }
        public string ContentType { get; set; }
        public string ExternalUrl { get; set; }
    }
}
