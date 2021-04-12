using Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class Comment
    {
        public Guid ID { get; set; }
        public string Hash { get; set; } = new Hash().NewHash(7);
        public Guid VoxID { get; set; }
        public Guid UserID { get; set; }
        public Guid? MediaID { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Bump { get; set; } = DateTimeOffset.Now;
        public CommentState State { get; set; }
        public Media Media { get; set; }
        //public Vox Vox { get; set; }
        public User User { get; set; }
        //public string[] Replies { get; set; }
    }

    public enum CommentState { Normal, Eliminado }
}
