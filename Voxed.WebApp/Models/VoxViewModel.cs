using Core.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public enum VoxState { Normal, Deleted }
    

    public class VoxViewModel
    {
        public Guid ID { get; set; }
        public string Hash { get; set; } = new Hash().NewHash();
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }        
        public Guid UserID { get; set; }
        public Guid MediaID { get; set; }
        public VoxState State { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Bump { get; set; } = DateTimeOffset.Now;
        public Media Media { get; set; }
        public Category Category { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
