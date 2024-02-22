using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Voxed.WebApp.Attributes;

namespace Voxed.WebApp.Models
{
    public static class Extensions
    {
        public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    }
    public class AttachmentRequest
    {
        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "El archivo supera los 20 MB permitidos.")]
        [AllowedExtensions(new []{ ".jpg", ".jpeg", ".png", ".gif", ".webp", ".webm", ".mp4", "m4v" }, ErrorMessage = "Formato de archivo no soportado.")]
        public IFormFile File { get; set; }
        public string UploadData { get; set; }

        public VoxedAttachment GetVoxedAttachment()
        {
            return JsonConvert.DeserializeObject<VoxedAttachment>(UploadData);
        }

        public bool HasAttachment()
        {
            return GetVoxedAttachment().HasData() || File is not null;
        }
    }
}
