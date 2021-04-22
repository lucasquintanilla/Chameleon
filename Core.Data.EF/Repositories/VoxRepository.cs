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
        //private readonly VoxedContext _context;

        public VoxRepository(VoxedContext context) : base(context)
        {
            //_context = context;
        }

        public override async Task<Vox> GetById(Guid id)
            => await _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Category.Media)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Media)
                .Include(c => c.Comments)
                    .ThenInclude(c => c.User)
                .Include(x => x.Poll)
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.ID == id);
        

        public async Task<IEnumerable<Vox>> GetLastestAsync() =>
            await _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Comments)
                //.OrderByDescending(x => x.Bump).ThenByDescending(x => x.Type)
                .OrderByDescending(x => x.Type).ThenByDescending(x => x.Bump)
                .Skip(0)
                .Take(36)
                .Where(x => x.State == VoxState.Normal || x.Type == VoxType.Sticky)
                .AsNoTracking()
                .ToListAsync();

        public override async Task<IEnumerable<Vox>> GetAll()
            => await _context.Voxs
                   .Include(x => x.Media)
                   .Include(x => x.Category)
                   .Include(x => x.Comments)
                   .Include(x => x.User)
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

        public async Task<IEnumerable<Vox>> SearchAsync(string search)
        {
            return await _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Bump)
                .Skip(0)
                .Take(36)
                .Where(x => x.State == VoxState.Normal)
                .Where(q =>  q.Title.ToLower().Contains(search.ToLower()))
                .Where(q => q.Content.ToLower().Contains(search.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            //var p = db.Posts.Where(q => keywords.Any(k => q.Title.Contains(k)));
        }

        public async Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<string> hashSkipList) =>        
            await _context.Voxs
                    .Include(x => x.Media)
                    .Include(x => x.Category)
                    .Include(x => x.Comments)
                    .OrderByDescending(x => x.Type).ThenByDescending(x => x.Bump)                    
                    .Where(x => x.State == VoxState.Normal)
                    .Where(x => !hashSkipList.Contains(x.Hash))
                    .Skip(0)
                    .Take(36)
                    .AsNoTracking()
                    .ToListAsync();

        public async Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<string> hashSkipList, DateTimeOffset LastBump) =>
            await _context.Voxs
                    .Where(x => x.State == VoxState.Normal)
                    .Where(x => !hashSkipList.Contains(x.Hash) && x.Bump < LastBump)
                    .OrderByDescending(x => x.Type).ThenByDescending(x => x.Bump)
                    .Include(x => x.Media)
                    .Include(x => x.Category)
                    .Include(x => x.Comments)                                    
                    .Skip(hashSkipList.Count())
                    .Take(36)
                    .AsNoTracking()
                    .ToListAsync();

        public async Task<Vox> GetByHash(string hash)
         => await _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Category.Media)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Media)
                .Include(c => c.Comments)
                    .ThenInclude(c => c.User)
                .Include(x => x.Poll)
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.Hash == hash);

        public async Task<IEnumerable<Vox>> GetByCategoryShortNameAsync(string shortName)
            => await _context.Voxs
                        .Where(x => x.Category.ShortName == shortName)
                        .Include(x => x.Media)
                        .Include(x => x.Category)
                        .Include(x => x.Comments)
                        .OrderByDescending(x => x.Bump)
                        .Skip(0)
                        .Take(100)
                        .ToListAsync();

        public async Task<Vox> GetLastVoxBump(IEnumerable<string> hash)
         => await _context.Voxs
            .Where(x => hash.Contains(x.Hash))
            .OrderBy(x => x.Bump)
            .FirstAsync();
    }
}
