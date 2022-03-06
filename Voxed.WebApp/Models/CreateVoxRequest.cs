﻿using Core.Shared.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voxed.WebApp.Models
{
    public class CreateVoxRequest
    {
        [Required(ErrorMessage = "Debe ingresar un titulo")]
        [StringLength(120, ErrorMessage = "El titulo no puede superar los {1} caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Debe ingresar un contenido")]
        [StringLength(5000, ErrorMessage = "El contenido no puede superar los {1} caracteres.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoria.")]
        public int Niche { get; set; }
        public IFormFile File { get; set; }
        public string PollOne { get; set; }
        public string PollTwo { get; set; }
        public string UploadData { get; set; }


        [JsonPropertyName("g-recaptcha-response")]
        public string GReCaptcha { get; set; }

        [JsonPropertyName("h-captcha-response")]
        public string HCaptcha { get; set; }

        public UploadData GetUploadData()
        {
            return JsonConvert.DeserializeObject<UploadData>(UploadData);
        }
    }
}
