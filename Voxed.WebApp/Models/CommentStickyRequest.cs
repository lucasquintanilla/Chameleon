using System;

namespace Voxed.WebApp.Models
{
    public class CommentStickyRequest
    {
        public string ContentType { get; set; }
        public Guid ContentId { get; set; }
        public string Vox { get; set; }
    }
}
