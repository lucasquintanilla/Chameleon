using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Shared
{
    public static class UserNameGenerator
    {
        private static readonly Random random = new Random();

        private const string AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

        private static string NewRandomString(int length = 20)
        {
            return new string(Enumerable.Repeat(AllowedUserNameCharacters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string NewAnonymousUserName()
        {
            return "anon" + NewRandomString(12);
        }
    }
}