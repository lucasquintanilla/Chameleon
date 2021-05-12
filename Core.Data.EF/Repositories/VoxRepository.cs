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
        private int[] hiddenCategoriesId = new int[] { 2, 3 };
        public VoxRepository(VoxedContext context) : base(context) { }

        public override async Task<Vox> GetById(Guid id)
            => await _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Category.Media)
                .Include(x => x.Comments.Where(c => c.State == CommentState.Normal)) //agregar order by descending
                    .ThenInclude(c => c.Media)
                .Include(x => x.Comments.Where(c => c.State == CommentState.Normal))
                    .ThenInclude(c => c.User)
                .Include(x => x.Poll)
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.ID == id);

        public async Task<IEnumerable<Vox>> GetLastestAsync() =>
            await _context.Voxs
                .Where(x => x.State == VoxState.Normal && !hiddenCategoriesId.Contains(x.CategoryID))
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Comments.Where(c => c.State == CommentState.Normal))
                .OrderByDescending(x => x.Type).ThenByDescending(x => x.Bump)
                .Skip(0)
                .Take(36)
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
                        .Where(x => x.CategoryID == id && x.State == VoxState.Normal)
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
                .Where(x => x.State == VoxState.Normal)
                .Where(q => q.Title.ToLower().Contains(search.ToLower()))
                .Where(q => q.Content.ToLower().Contains(search.ToLower()))
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Bump)                
                .Skip(0)
                .Take(36)
                .AsNoTracking()
                .ToListAsync();

            //var p = db.Posts.Where(q => keywords.Any(k => q.Title.Contains(k)));
        }

        public async Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<string> hashSkipList) =>        
            await _context.Voxs
                    .Include(x => x.Media)
                    .Include(x => x.Category)
                    .Include(x => x.Comments.Where(c => c.State == CommentState.Normal))
                    .OrderByDescending(x => x.Type).ThenByDescending(x => x.Bump)                    
                    .Where(x => x.State == VoxState.Normal)
                    .Where(x => !hashSkipList.Contains(x.Hash))
                    .Skip(0)
                    .Take(36)
                    .AsNoTracking()
                    .ToListAsync();

        public async Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<string> hashSkipList, DateTimeOffset LastBump) =>
            await _context.Voxs
                    .Where(x => x.State == VoxState.Normal 
                                && x.Type == VoxType.Normal 
                                && !hashSkipList.Contains(x.Hash) 
                                && x.Bump < LastBump 
                                && !hiddenCategoriesId.Contains(x.CategoryID))
                    .OrderByDescending(x => x.Bump)
                    .Include(x => x.Media)
                    .Include(x => x.Category)
                    .Include(x => x.Comments.Where(c => c.State == CommentState.Normal))                                    
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
                        .Where(x => x.Category.ShortName == shortName && x.State == VoxState.Normal)
                        .Include(x => x.Media)
                        .Include(x => x.Category)
                        .Include(x => x.Comments)
                        .OrderByDescending(x => x.Bump)
                        .Skip(0)
                        .Take(100)
                        .ToListAsync();
        //agregar as no tracking

        public async Task<Vox> GetLastVoxBump(IEnumerable<string> hash)
         => await _context.Voxs
            .Where(x => hash.Contains(x.Hash))
            .OrderBy(x => x.Bump)
            .FirstAsync();
        //agregar as no tracking
    }
}
