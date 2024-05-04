using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(BlogContext context) : base(context) { }

        public async Task<bool> Exists(int id)
        {
            return await Entities.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> Exists(string shortName)
        {
            return await Entities.AnyAsync(c => c.ShortName == shortName);
        }

        public async Task<Category> GetByShortName(string shortName)
        {
            return await Entities.Where(c => c.ShortName == shortName).SingleOrDefaultAsync();
        }

        public override async Task<IEnumerable<Category>> GetAll()
        {
            return await Entities
                .OrderBy(c => c.Name)
                .Include(c => c.Media)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Category> GetById(int id)
        {
            return await Entities
                .Include(c => c.Media)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
