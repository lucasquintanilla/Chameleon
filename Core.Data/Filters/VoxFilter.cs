using System;
using System.Collections.Generic;

namespace Core.Data.Filters
{
    public class VoxFilter
    {
        public Guid? UserId { get; set; }
        public IEnumerable<Guid> IgnoreVoxIds { get; set; } = new List<Guid>();
        public IEnumerable<int> Categories { get; set; } = new List<int>();
        public IEnumerable<string> HiddenWords { get; set; } = new List<string>();
        public string Search { get; set; }
        public bool IncludeHidden { get; set; }
        public bool IncludeFavorites { get; set; }
    }
}
