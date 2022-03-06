using System.Collections.Generic;

namespace Voxed.WebApp.Models
{
    public class ListResponse
    {
        public bool Status { get; set; }
        public List List { get; set; }
    }

    public class List
    {
        public IEnumerable<VoxResponse> Voxs { get; set; } = new List<VoxResponse>();
        public string Page { get; set; }
    }
}
