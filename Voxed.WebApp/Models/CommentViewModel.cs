using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class CommentViewModel
    {
        public Guid ID { get; set; }
        public string Hash { get; set; }
        //public Guid VoxID { get; set; }
        public Guid UserID { get; set; }
        //public Guid? MediaID { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        //public DateTimeOffset Bump { get; set; }
        //public CommentState State { get; set; }
        public MediaViewModel Media { get; set; }
        //public User User { get; set; }
    }
}
