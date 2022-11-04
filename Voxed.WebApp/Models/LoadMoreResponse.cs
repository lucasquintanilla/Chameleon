using System.Collections.Generic;
using System.Linq;

namespace Voxed.WebApp.Models
{
    public class LoadMoreResponse : BaseResponse
    {
        public LoadMoreResponse(IList<VoxResponse> voxsList)
        {
            Status = voxsList.Any();
            List = new List()
            {
                Page = "category-sld",
                Voxs = voxsList
            };
        }

        public List List { get; set; }
    }

    public class List
    {
        public IEnumerable<VoxResponse> Voxs { get; set; } = new List<VoxResponse>();
        public string Page { get; set; }
    }
}
