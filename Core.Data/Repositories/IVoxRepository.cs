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
        Task<IEnumerable<Vox>> GetByCategoryIdAsync(int id);
        Task<IEnumerable<Vox>> SearchAsync(string search);
    }
}
