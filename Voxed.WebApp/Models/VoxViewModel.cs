using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class VoxViewModel
    {
        public Guid ID { get; set; }
        public string Hash { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }
        public Guid UserID { get; set; }
        public Guid MediaID { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public MediaViewModel Media { get; set; }
        public CategoryViewModel Category { get; set; }
        public User User { get; set; }
        public ICollection<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }
}
