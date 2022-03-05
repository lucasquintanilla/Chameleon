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
        public static string GetFileExtension(this Image image)
        {
            if (image.RawFormat.Equals(ImageFormat.Jpeg))
            {
                return ".jpg";
            }
            if (image.RawFormat.Equals(ImageFormat.Gif))
            {
                return ".gif";
            }
            if (image.RawFormat.Equals(ImageFormat.Png))
            {
                return ".png";
            }
            if (image.RawFormat.Equals(ImageFormat.Bmp))
            {
                return ".bmp";
            }

            throw new NotImplementedException("Formato archivo no implementado");
        }

        public static ImageFormat GetImageFormat(this Image image)
        {
            if (image.RawFormat.Equals(ImageFormat.Jpeg))
            {
                return ImageFormat.Jpeg;
            }

            if (image.RawFormat.Equals(ImageFormat.Gif))
            {
                return ImageFormat.Gif;
            }
            if (image.RawFormat.Equals(ImageFormat.Png))
            {
                return ImageFormat.Png;
            }
            if (image.RawFormat.Equals(ImageFormat.Bmp))
            {
                return ImageFormat.Bmp;
            }

            throw new NotImplementedException("Formato archivo no implementado");
        }

        public static string GetFileExtension(this IFormFile file)
        {
            return Path.GetExtension(file.FileName).ToLowerInvariant();
        }

        public static bool IsGif(this IFormFile file)
            => file.ContentType == MediaTypeNames.Image.Gif;
    }
}
