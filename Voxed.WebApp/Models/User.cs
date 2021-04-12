using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public enum UserType { Admin, Anon, Mod }


    public class User
    {
        public Guid ID { get; set; }
        public string Username { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public UserType UserType { get; set; }
    }
}
