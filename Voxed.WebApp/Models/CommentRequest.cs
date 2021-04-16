using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class CommentRequest
    {
        public string Content { get; set; }
        public IFormFile File { get; set; }
        public string UploadData { get; set; }
    }
}
