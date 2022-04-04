using System;

namespace Core.Entities
{
    public class UserVoxAction : Entity
    {
        public Guid UserId { get; set; }
        public Guid VoxId { get; set; }
        public bool IsFollowed { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsHidden { get; set; }
        public virtual User User { get; set; }
        public virtual Vox Vox { get; set; }
    }
}
