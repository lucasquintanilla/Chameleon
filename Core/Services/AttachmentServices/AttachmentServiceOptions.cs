using System.Collections.Generic;

namespace Core.Services.AttachmentServices
{
    public class AttachmentServiceOptions
    {
        public const string SectionName = "AttachmentService";

        public IEnumerable<string> PermittedExtensions { get; set; }
    }
}
