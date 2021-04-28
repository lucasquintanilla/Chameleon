using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public enum CommentState { Normal, Deleted }
    public enum CommentType { Normal, Sticky }
    public enum CommentStyle { Black, Blue, Green, Multi, Red, White, Yellow } //invested

    public class Comment
    {
        public Guid ID { get; set; }
        public string Hash { get; set; }
        public Guid VoxID { get; set; }
        public Guid UserID { get; set; }
        public Guid? MediaID { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Bump { get; set; } = DateTimeOffset.Now;
        public CommentState State { get; set; }
        public CommentType Type { get; set; }
        public CommentStyle Style { get; set; }
        public Media Media { get; set; }
        public User User { get; set; }
        //public ICollection<Comment> Replies { get; set; }
    }
}
