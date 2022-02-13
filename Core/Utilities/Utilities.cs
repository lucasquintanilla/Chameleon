using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities
{
    public static class Utilities
    {
        public static bool IsDebug()
        {
#if DEBUG
            return true;
#endif
            return false;

        }
    }
}
