using Core.DataSources.Devox.Models;

namespace Core.DataSources.Devox.Helpers
{
    public static class DevoxHelpers
    {
        public static string GetThumbnailUrl(DevoxPost vox)
        {
            if (vox.IsURL)
            {
                if (vox.FileExtension == "video")
                    return $"https://img.youtube.com/vi/{vox.Url}/0.jpg";

                return vox.Url;
            }

            //return $"https://{Constants.Domain}/backgrounds/low-res_{vox.Filename}.jpeg";
            return $"{Constants.CdnUrl}/file/backgr/low-res_{vox.Filename}.jpeg";
        }
    }
}
