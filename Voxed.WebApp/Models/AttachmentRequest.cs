using Core.Extensions;
using Core.Services.AttachmentServices.Models;
using Core.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace Voxed.WebApp.Models
{
    public class AttachmentRequest
    {
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "El archivo supera los 10 MB permitidos.")]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }, ErrorMessage = "Formato de archivo no soportado.")]
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
