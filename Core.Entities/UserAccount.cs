using System;

namespace Core.Entities
{
    internal class UserAccount : Entity
    {
        public Guid UserId { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
        public string Token { get; set; }
        public virtual User Owner { get; set; }
    }
}
