using Core.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public enum VoxState { Normal, Deleted }
    public enum VoxType { Normal, Sticky }

    public class Vox
    {
        public Guid ID { get; set; }
        public string Hash { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }        
        public Guid UserID { get; set; }
        public Guid MediaID { get; set; }
        public VoxState State { get; set; }
        public VoxType Type { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Bump { get; set; } = DateTimeOffset.Now;
        public Media Media { get; set; }
        public Category Category { get; set; }
        public virtual User User { get; set; }
        public Poll Poll { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
