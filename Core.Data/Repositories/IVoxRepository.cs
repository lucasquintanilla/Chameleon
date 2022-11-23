using Core.Data.Filters;
using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IVoxRepository : IRepository<Vox>
    {
        Task<List<Vox>> GetByFilterAsync(PostFilter filter);
    }
}
