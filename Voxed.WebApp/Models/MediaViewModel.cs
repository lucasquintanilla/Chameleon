using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class MediaViewModel
    {
        public Guid ID { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public MediaType MediaType { get; set; }
    }
}
