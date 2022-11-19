using System.Collections.Generic;

namespace Core.Services.AttachmentServices
{
    public class AttachmentServiceConfiguration
    {
        public const string SectionName = "AttachmentService";

        public IEnumerable<string> PermittedExtensions { get; set; }
    }
}
