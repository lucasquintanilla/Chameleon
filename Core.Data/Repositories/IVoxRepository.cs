using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IVoxRepository : IGenericRepository<Vox>
    {
        Task<IEnumerable<Vox>> GetLastestAsync();
        //Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<string> hashSkipList);
        //Task<IEnumerable<Vox>> GetByCategoryIdAsync(int id);
        Task<IEnumerable<Vox>> GetByCategoryShortNameAsync(string shortName);
        Task<IEnumerable<Vox>> SearchAsync(string search);
        //Task<Vox> GetByHash(string hash);
        //Task<Vox> GetLastVoxBump(IEnumerable<string> hash);
        //Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<string> hashSkipList, DateTimeOffset LastBump);
        Task<Vox> GetLastVoxBump(IEnumerable<Guid> idsSkip);
        Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<Guid> idSkipList, DateTimeOffset LastBump);
    }
}
