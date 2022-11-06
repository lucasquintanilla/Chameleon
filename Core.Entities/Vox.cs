using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public enum VoxState { Active, Deleted, Reported }

    public class Vox : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public Guid UserId { get; set; }
        public Guid AttachmentId { get; set; }
        public VoxState State { get; set; }
        public bool IsSticky { get; set; }
        public DateTimeOffset Bump { get; set; } = DateTimeOffset.Now;
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
        public virtual Attachment Attachment { get; set; }
        public virtual Category Category { get; set; }
        public virtual User Owner { get; set; }
        public virtual Poll Poll { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();        
    }
}
