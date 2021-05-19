using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Shared
{
    public static class UserViewHelper
    {
        public static string GetUserName(User user)
        {
            switch (user.UserType)
            {
                case UserType.Anonymous:
                    return "Anonimo";
                case UserType.Administrator:
                    return user.UserName;
                case UserType.Moderator:
                    return user.UserName;
                case UserType.Account:
                    return user.UserName;
                case UserType.AnonymousAccount:
                    return "Anonimo";
                default:
                    throw new NotImplementedException("Tipo de usuario no contemplado");
            }
        }

        public static string GetUserTypeTag(UserType userType)
        {
            switch (userType)
            {
                case UserType.Anonymous:
                    return "anon";
                case UserType.Administrator:
                    return "admin";
                case UserType.Moderator:
                    return "mod";
                case UserType.Account:
                    return "anon";
                default:
                    return "anon";
            }
        }
    }
}
