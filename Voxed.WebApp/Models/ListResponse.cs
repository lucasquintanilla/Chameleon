using System.Collections.Generic;

namespace Voxed.WebApp.Models
{
    public class ListResponse : BaseResponse
    {
        public List List { get; set; }
    }

    public class List
    {
        public IEnumerable<VoxResponse> Voxs { get; set; } = new List<VoxResponse>();
        public string Page { get; set; }
    }
}
