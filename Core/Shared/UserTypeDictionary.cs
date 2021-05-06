﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
                    };

                return _description;
            }
        }

        public static string GetDescription(UserType code)
            => Description[code];
    }
}
