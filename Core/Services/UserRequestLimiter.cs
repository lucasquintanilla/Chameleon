using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    internal class UserActivity
    {
        private static readonly Dictionary<Guid, DateTimeOffset> _userLastActivityOn = new();

        public bool CanUserCreateComment(Guid userId)
        {
            if (_userLastActivityOn.ContainsKey(userId))
            {
                return HasUserCompletedTime(userId);
            }

            return true;            
        }

        private static bool HasUserCompletedTime(Guid userId)
        {
            var interval = TimeSpan.FromMinutes(2);
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
