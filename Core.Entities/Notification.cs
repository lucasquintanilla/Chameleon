using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public enum NotificationType
    {
        Comment, Response
    }

    public class Notification
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CommentId { get; set; }
        public NotificationType Type { get; set; }
        public int Count { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public virtual Vox Vox { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
