using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public enum VoxState { Normal, Deleted }

    public class Vox : IAttachment
    {
        public Guid ID { get; set; }
        public string Hash { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }
        public Guid UserID { get; set; }
        public Guid MediaID { get; set; }
        public VoxState State { get; set; }
        public bool IsSticky { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Bump { get; set; } = DateTimeOffset.Now;
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
        public virtual Media Media { get; set; }
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
        public virtual Poll Poll { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        
    }
}
