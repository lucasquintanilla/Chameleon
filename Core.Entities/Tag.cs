using System.Collections.Generic;

namespace Core.Entities
{
    public interface ITaggable
    {
        ICollection<Tag> Tags { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
