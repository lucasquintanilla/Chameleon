using System;
using System.Collections.Generic;

namespace Core.Data.Filters
{
    public class VoxFilter
    {
        public Guid? UserId { get; set; }
        public List<Guid> IgnoreVoxIds { get; set; } = new List<Guid>();
        public List<int> Categories { get; set; } = new List<int>();
        public List<string> HiddenWords { get; set; } = new List<string>();
        public string Search { get; set; }
        public bool IncludeHidden { get; set; }
        public bool IncludeFavorites { get; set; }
    }
}
