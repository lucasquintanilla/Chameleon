using Core.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Voxed.WebApp.Models
{
    public class CommentFormViewModel
    {
        public Guid VoxID { get; set; }
        public Guid UserID { get; set; }

        [Required(ErrorMessage = "Debe ingresar un comentario")]
        [StringLength(3000, ErrorMessage = "El comentario no puede superar los {1} caracteres.")]
        public string Content { get; set; }

        [MaxFileSize(10 * 1024 * 1024)]
        //[AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".webm" }, ErrorMessage = "Formato de archivo no soportado.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }, ErrorMessage = "Formato de archivo no soportado.")]
        public IFormFile File { get; set; }
    }
}
