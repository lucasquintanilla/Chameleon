using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class VoxRepository : GenericRepository<Vox>, IVoxRepository
    {
        private readonly VoxedContext _context;

        public VoxRepository(VoxedContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<Vox> GetById(Guid id)
            => await _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Category.Media)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Media)
                .Include(x => x.Poll)
                .FirstOrDefaultAsync(m => m.ID == id);
        

        public async Task<IEnumerable<Vox>> GetLastestAsync() =>
            await _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Bump)
                .Skip(0)
                .Take(50)
                .Where(x => x.State == VoxState.Normal)
                .AsNoTracking()
                .ToListAsync();

        public override async Task<IEnumerable<Vox>> GetAll()
            => await _context.Voxs
                   .Include(x => x.Media)
                   .Include(x => x.Category)
                   .Include(x => x.Comments)                   
                   .ToListAsync();

        public async Task<IEnumerable<Vox>> GetByCategoryIdAsync(int id)
            => await _context.Voxs
                        .Where(x => x.CategoryID == id)
                        .Include(x => x.Media)
                        .Include(x => x.Category)
                        .Include(x => x.Comments)
                        .OrderByDescending(x => x.Bump)
                        .Skip(0)
                        .Take(100)
                        .ToListAsync();
    }
}
