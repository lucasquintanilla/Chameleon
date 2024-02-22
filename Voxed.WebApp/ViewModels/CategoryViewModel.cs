using Core.Entities;
using Core.Utilities;
using System.Collections.Generic;

namespace Voxed.WebApp.ViewModels
{
    public class CategoryViewModel : IDefaultCollection<Category>
    {
        public Category Default { get; set; }
        public IEnumerable<Category> Values { get; set; }
    }
}
