using System;

namespace Core.Shared
{
    public class GuidConverter
    {
        public static string ToShortString(Guid guid)
        {
            var base64Guid = Convert.ToBase64String(guid.ToByteArray());

            // Replace URL unfriendly characters with better ones
            base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

            // Remove the trailing ==
            return base64Guid.Substring(0, base64Guid.Length - 2);
        }

        public static Guid FromShortString(string base64Guid)
        {
            base64Guid = base64Guid.Replace('_', '/').Replace('-', '+');
            var byteArray = Convert.FromBase64String(base64Guid + "==");
            return new Guid(byteArray);
        }
    }
}
