using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities
{
    public static class Utilities
    {
        public static string GetFileExtensionFromUrl(string url)
        {
            var array = url.Split(".");
            return array[array.Length - 1];
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#endif
            return false;

        }
    }
}
