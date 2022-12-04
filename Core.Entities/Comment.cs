using System;

namespace Core.Entities
{
    public enum CommentState { Active, Deleted, Reported }
    public enum CommentType { Normal, Sticky }
    public enum AvatarStyle { Black, Blue, Green, Multi, Red, White, Yellow } //invested

    public class Comment : Entity
    {
        public string Hash { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public Guid? AttachmentId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Bump { get; set; } = DateTimeOffset.Now;
        public CommentState State { get; set; }
        public CommentType Type { get; set; }
        public AvatarStyle Style { get; set; }
        public bool IsSticky { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
        public Media Attachment { get; set; }
        public User Owner { get; set; }
    }
}
