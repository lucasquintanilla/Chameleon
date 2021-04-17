using Core.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class CommentRequest
    {
        [Required(ErrorMessage = "Debe ingresar un comentario")]
        [StringLength(3000, ErrorMessage = "El comentario no puede superar los {1} caracteres.")]
        public string Content { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "El archivo supera los 10 MB permitidos.")]
        //[AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".webm" }, ErrorMessage = "Formato de archivo no soportado.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }, ErrorMessage = "Formato de archivo no soportado.")]
        public IFormFile File { get; set; }

        public string UploadData { get; set; }
    }
}
