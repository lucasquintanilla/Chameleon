using Core.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class VoxFormViewModel
    {
        [Required(ErrorMessage = "Debe ingresar un titulo")]
        [StringLength(120, ErrorMessage = "El titulo no puede superar los {1} caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Debe ingresar un contenido")]
        [StringLength(5000, ErrorMessage = "El contenido no puede superar los {1} caracteres.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoria.")]
        public int CategoryID { get; set; }
        public Guid UserID { get; set; }

        [Required(ErrorMessage = "Debe agregar un archivo")]
        [MaxFileSize(10 * 1024 * 1024)]
        //[AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".webm" }, ErrorMessage = "Formato de archivo no soportado.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }, ErrorMessage = "Formato de archivo no soportado.")]
        //[FileExtensions(Extensions = ".jpg,.jpeg,.png,.gif", ErrorMessage = "Formato de archivo no soportado.")]
        //[FileExtensions]
        public IFormFile File { get; set; }
        public string MediaUrl { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public PollViewModel Poll { get; set; }
    }
}
