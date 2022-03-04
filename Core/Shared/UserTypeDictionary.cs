using Core.Entities;
using System.Collections.Generic;

namespace Core.Shared
{
    public class UserTypeDictionary
    {
        private static Dictionary<UserType, string> _description;

        public static Dictionary<UserType, string> Description
        {
            get
            {
                if (_description == null)
                    _description = new Dictionary<UserType, string>()
                    {
                        {UserType.Anonymous, "anon" },
                        {UserType.Administrator, "admin" },
                        {UserType.Moderator, "mod" },
                        {UserType.Account, "anon" },
                        {UserType.AnonymousAccount, "anon" },
                    };

                return _description;
            }
        }

        public static string GetDescription(UserType code)
            => Description[code];
    }
}
