using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Core.Extensions
{
    public static class Extensions
    {
        public static string GetFileExtension(this IFormFile file)
        {
            return Path.GetExtension(file.FileName).ToLowerInvariant();
        }

        public static bool IsNew(this DateTimeOffset dateTime)
        {
            return dateTime.Date > DateTime.Now.Date.AddHours(-24);
        }
    }
}
