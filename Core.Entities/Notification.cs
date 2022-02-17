﻿using System;

namespace Core.Entities
{
    public enum NotificationType { NewComment, Reply }

    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid VoxId { get; set; }
        public Guid CommentId { get; set; }
        public NotificationType Type { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public virtual User Owner { get; set; }
        public virtual Vox Vox { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
