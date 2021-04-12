using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Shared
{
    public class Hash
    {
        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string NewHash(int length = 20)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    //public struct Hash
    //{
    //    private static readonly Random random = new Random();
    //    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";        

    //    public static string NewHash(int length = 20)
    //    {
    //        return new string(Enumerable.Repeat(chars, length)
    //          .Select(s => s[random.Next(s.Length)]).ToArray());
    //    }

    //    public override string ToString() => "jeje";
    //}
}
