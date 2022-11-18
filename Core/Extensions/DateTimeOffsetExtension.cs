using System;

namespace Core.Extensions
{
    public static class DateTimeOffsetExtension
    {
        public static bool IsNew(this DateTimeOffset dateTime)
        {
            return dateTime.Date > DateTime.Now.Date.AddHours(-24);
        }
    }
}
