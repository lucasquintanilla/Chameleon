using System.Security.AccessControl;

namespace Voxed.WebApp.ViewModels
{
    public enum MediaType { Image, Video, YouTube, Gif }

    public class MediaViewModel
    {
        public string Url { get; set; }
        public MediaType MediaType { get; set; }
        public string ExtensionData { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
