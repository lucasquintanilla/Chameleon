using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Data.EF
{
    public enum RoleType
    {
        Administrator,
        Moderator,    
        Anonymous
    }

    public static class RolesHelper
    {
        public static Dictionary<RoleType, string> RolesDictionary = new Dictionary<RoleType, string>()
        {
            { RoleType.Administrator, nameof(RoleType.Administrator) },
            { RoleType.Moderator, nameof(RoleType.Moderator) },
            { RoleType.Anonymous, nameof(RoleType.Anonymous) },
        };
    }
}
