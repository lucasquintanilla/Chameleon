using System;

namespace Core.Entities
{
    public class UserVoxAction : Entity
    {
        public Guid UserId { get; set; }
        public Guid VoxId { get; set; }
        public bool Follow { get; set; }
        public bool Favorite { get; set; }
        public bool Hide { get; set; }
        public virtual User User { get; set; }
        public virtual Vox Vox { get; set; }
    }
}
