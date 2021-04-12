using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class CategoryViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        //public Guid MediaID { get; set; }
        public MediaViewModel Media { get; set; }

    }
}
