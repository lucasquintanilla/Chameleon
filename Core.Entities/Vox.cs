using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public enum VoxState { Normal, Deleted }

    public class Vox : IMediaEntity
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
        public Media Media { get; set; }
        public Category Category { get; set; }
        public virtual User User { get; set; }
        public Poll Poll { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
    }
}
