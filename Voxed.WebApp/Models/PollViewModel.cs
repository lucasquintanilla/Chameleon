using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class PollViewModel
    {
        [StringLength(50, ErrorMessage = "El contenido no puede superar los {1} caracteres.")]
        public string OptionADescription { get; set; }

        [StringLength(50, ErrorMessage = "El contenido no puede superar los {1} caracteres.")]
        public string OptionBDescription { get; set; }
        public int OptionAVotes { get; set; }
        public int OptionBVotes { get; set; }
    }
}
