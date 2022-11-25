using System.Collections.Generic;

namespace Core.Services.MediaServices
{
    public class MediaServiceOptions
    {
        public const string SectionName = "MediaService";

        public IEnumerable<string> PermittedExtensions { get; set; }
    }
}
