﻿using System;

namespace Core.Entities
{
    public enum NotificationType { NewComment, Reply }

    public class Notification : Entity
    {
        public Guid UserId { get; set; }
        public Guid VoxId { get; set; }
        public Guid CommentId { get; set; }
        public NotificationType Type { get; set; }
        public virtual User Owner { get; set; }
        public virtual Vox Vox { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
