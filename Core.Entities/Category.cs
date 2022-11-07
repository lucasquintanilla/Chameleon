using System;

namespace Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Nsfw { get; set; }
        public Guid AttachmentId { get; set; }
        public Attachment Attachment { get; set; }
    }
}
