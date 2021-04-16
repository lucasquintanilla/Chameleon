using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class CommentResponse
    {
        public bool Status { get; set; }
        public string  Hash { get; set; }
        public string Swal { get; set; }
        public string Error { get; set; }
    }
}
