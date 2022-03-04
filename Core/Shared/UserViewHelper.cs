using Core.Entities;
using System;

namespace Core.Shared
{
    public static class UserViewHelper
    {
        public static string GetUserName(User user)
        {
            return user.UserType switch
            {
                UserType.Anonymous => "Anonimo",
                UserType.Administrator => user.UserName,
                UserType.Moderator => user.UserName,
                UserType.Account => user.UserName,
                UserType.AnonymousAccount => "Anonimo",
                _ => throw new NotImplementedException("Tipo de usuario no contemplado"),
            };
        }

        public static string GetUserTypeTag(UserType userType)
        {
            return userType switch
            {
                UserType.Anonymous => "anon",
                UserType.Administrator => "admin",
                UserType.Moderator => "mod",
                UserType.Account => "anon",
                _ => "anon"
            };
        }
    }
}
