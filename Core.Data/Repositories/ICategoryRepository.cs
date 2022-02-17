using Core.Entities;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> Exists(int id);
        Task<bool> Exists(string category);
    }
}
