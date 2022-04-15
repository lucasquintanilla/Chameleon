using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IVoxRepository : IGenericRepository<Vox>
    {
        Task<IEnumerable<Vox>> GetLastestAsync(ICollection<int> includedCategories, IEnumerable<Guid> idSkipList);
        Task<IEnumerable<Vox>> GetByCategoryShortNameAsync(string shortName);
        Task<IEnumerable<Vox>> SearchAsync(string search);
        Task<Vox> GetLastVoxBump(IEnumerable<Guid> idsSkip);
        Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<Guid> idSkipList, DateTimeOffset LastBump, ICollection<int> excludedCategories);
        Task<IEnumerable<Vox>> GetFavoritesAsync(Guid userId);
        Task<IEnumerable<Vox>> GetHiddenAsync(Guid userId);
    }
}
