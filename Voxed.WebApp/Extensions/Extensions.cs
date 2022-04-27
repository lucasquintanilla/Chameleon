using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;

namespace Voxed.WebApp.Extensions
{
    public static class Extensions
    {
        public static string ToTimeAgo(this DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return $"{years}y";
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return $"{months}M";
            }
            if (span.Days > 0)
                return $"{span.Days}d";
            if (span.Hours > 0)
                return $"{span.Hours}h";
            if (span.Minutes > 0)
                return $"{span.Minutes}m";
            if (span.Seconds > 1)
                return $"{span.Seconds}s";
            if (span.Seconds <= 0)
                return "Ahora";
            return "Ahora";
        }

        public static string GetFileExtension(this IFormFile file)
        {
            return Path.GetExtension(file.FileName).ToLowerInvariant();
        }

        public static bool IsGif(this IFormFile file)
            => file.ContentType == MediaTypeNames.Image.Gif;

        public static bool IsNew(this DateTimeOffset dateTime)
        {
            return dateTime.Date > DateTime.Now.Date.AddHours(-24);
        }
    }
}
