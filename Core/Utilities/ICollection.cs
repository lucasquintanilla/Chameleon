using System.Collections.Generic;

namespace Core.Utilities
{
    public interface IDefaultCollection<T>
    {
        T Default { get; set; }
        IEnumerable<T> Values { get; set; }

    }
}
