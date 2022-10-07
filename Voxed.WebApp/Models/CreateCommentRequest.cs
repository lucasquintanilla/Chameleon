using Core.Shared.Models;
using Core.Validations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Voxed.WebApp.Models
{
    public class CreateCommentRequest
    {
        [StringLength(3000, ErrorMessage = "El comentario no puede superar los {1} caracteres.")]
        public string Content { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "El archivo supera los 10 MB permitidos.")]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }, ErrorMessage = "Formato de archivo no soportado.")]
        public IFormFile File { get; set; }
        public string UploadData { get; set; }

        public UploadData GetUploadData()
        {
            return JsonConvert.DeserializeObject<UploadData>(UploadData);
        }

        public bool HasEmptyContent()
        {
            return Content == null && File == null && GetUploadData()?.Extension == null;
        }
    }
}
