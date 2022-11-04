using Core.Extensions;
using Core.Shared.Models;
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

        public IFormFile GetFormFile()
        {
            if (File is not null)
            {
                return File;
            }

            var attachment = JsonConvert.DeserializeObject<VoxedAttachment>(UploadData);
            var stream = attachment.ExtensionData.GetStreamFromBase64();
            var dic = new Dictionary<string, StringValues>();
            dic.Add("Content-Type", "image/jpg");
            dic.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"image.jpg\"");
            return new FormFile(stream, 0, stream.Length, "file", $"{Guid.NewGuid()}.jpg")
            {
                Headers = new HeaderDictionary(dic),
                ContentType = MediaTypeNames.Image.Jpeg
            };
        }
    }
}
