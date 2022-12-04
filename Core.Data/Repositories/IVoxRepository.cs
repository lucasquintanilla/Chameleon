using Core.Data.Filters;
using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IVoxRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetByFilterAsync(PostFilter filter);
    }
}
