namespace Core.Entities
{
    public enum MediaType { Image, Video, YouTube, Gif }

    public class Media : Entity
    {
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public MediaType MediaType { get; set; }
    }
}
