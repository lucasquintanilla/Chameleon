using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IVoxRepository : IGenericRepository<Vox>
    {
        Task<IEnumerable<Vox>> GetLastestAsync();
        Task<IEnumerable<Vox>> GetByCategoryShortNameAsync(string shortName);
        Task<IEnumerable<Vox>> SearchAsync(string search);
        Task<Vox> GetLastVoxBump(IEnumerable<Guid> idsSkip);
        Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<Guid> idSkipList, DateTimeOffset LastBump);
    }
}
