using System;
using System.Collections.Generic;

namespace Core.Services
{
    public static class UserActivityLimiter
    {
        private static readonly Dictionary<Guid, DateTimeOffset> _userLastActivityOn = new();
        private const int Interval = 2;

        public static bool CanUserCreateComment(Guid userId)
        {
            if (_userLastActivityOn.ContainsKey(userId))
                return HasUserCompletedTime(userId);

            _userLastActivityOn.Add(userId, DateTimeOffset.UtcNow);
            return true;
        }

        private static bool HasUserCompletedTime(Guid userId)
        {
            var interval = TimeSpan.FromMinutes(Interval);
            var lastActivityOn = _userLastActivityOn[userId];
            if (DateTimeOffset.UtcNow - lastActivityOn > interval)
            {
                _userLastActivityOn.Remove(userId);
                return true;
            }
            return false;
        }
    }
}
