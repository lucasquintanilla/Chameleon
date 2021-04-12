using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public enum UserType { Admin, Anon, Mod }


    public class User : IdentityUser<Guid>
    //public class User
    {
        //public User()
        //{
        //}

        //public User(string userName) : base(userName)
        //{
        //}

        //public User(Guid id, string userName) : base(userName)
        //{
        //    Id = id;
        //    UserName = userName;
        //}

        //public Guid ID { get; set; } //borrar
        //public string Username { get; set; } //borrar
        public DateTimeOffset CreatedOn { get; set; }
        public UserType UserType { get; set; }
    }
}
