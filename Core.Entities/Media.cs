namespace Core.Entities
{
    public enum MediaType { Image, Video, YouTube, Gif }

    public class Media : Entity
    {
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; } //Eliminar
        public MediaType Type { get; set; }
        public string Key { get; set; }
        public string ThumbnailKey { get; set; } //Eliminar
        public string ContentType { get; set; }
        public string ExternalUrl { get; set; }
    }
}
