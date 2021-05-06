using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public enum UserType { Anonymous, Administrator, Moderator, Account }

    public class User : IdentityUser<Guid>
    {
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public UserType UserType { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
    }
}
