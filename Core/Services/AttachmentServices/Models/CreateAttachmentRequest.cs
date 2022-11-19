using Microsoft.AspNetCore.Http;

namespace Core.Services.AttachmentServices.Models
{
    public class CreateAttachmentRequest
    {
        public string Extension { get; set; }
        public string ExtensionData { get; set; }
        public IFormFile File { get; set; }
    }
}
