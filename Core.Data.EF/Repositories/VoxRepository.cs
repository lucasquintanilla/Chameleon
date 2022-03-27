using Core.Data.EF.Extensions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class VoxRepository : GenericRepository<Vox>, IVoxRepository
    {
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

        public async Task<IEnumerable<Vox>> GetLastestAsync(ICollection<int> includedCategories) =>
            await _context.Voxs
                .Where(x => x.State == VoxState.Normal && includedCategories.Contains(x.CategoryID))
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Comments.Where(c => c.State == CommentState.Normal))
                .OrderByDescending(x => x.IsSticky).ThenByDescending(x => x.Bump)
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

        public async Task<IEnumerable<Vox>> SearchAsync(string search)
        {
            HashSet<string> keywords = new HashSet<string>(search.ToLower().Split(' '));

            var predicateTitle = keywords.Select(k => (Expression<Func<Vox, bool>>)(x => x.Title.Contains(k))).ToArray();
            //var predicateContent = keywords.Select(k => (Expression<Func<Vox, bool>>)(x => x.Content.Contains(k))).ToArray();            

            var voxs = _context.Voxs
                .Include(x => x.Media)
                .Include(x => x.Category)
                .Include(x => x.Comments)
                .Where(x => x.State == VoxState.Normal)
                //.Where(vox => keywords.Any(keyword => vox.Title.Contains(keyword)))
                //.Where(vox => keywords.Any(keyword => vox.Content.Contains(keyword)))
                .WhereAny(predicateTitle)
                //.WhereAny(predicateContent)                
                .OrderByDescending(x => x.Bump)
                .Skip(0)
                .Take(36)
                .AsNoTracking()
                .ToList(); // WhereAny() No funciona con llamadas Async()

            return await Task.FromResult(voxs);
        }

        public async Task<IEnumerable<Vox>> GetLastestAsync(IEnumerable<Guid> idSkipList, DateTimeOffset lastBump, ICollection<int> includedCategories) =>
            await _context.Voxs
                    .Where(x => x.State == VoxState.Normal
                                //&& x.Type == VoxType.Normal
                                && !idSkipList.Contains(x.ID)
                                && x.Bump < lastBump
                                && includedCategories.Contains(x.CategoryID))
                    .OrderByDescending(x => x.Bump)
                    .Include(x => x.Media)
                    .Include(x => x.Category)
                    .Include(x => x.Comments.Where(c => c.State == CommentState.Normal))
                    //.Skip(idSkipList.Count()) // esto estab incluido peor no se porque
                    .Skip(0)
                    .Take(36)
                    .AsNoTracking()
                    .ToListAsync();

        public async Task<IEnumerable<Vox>> GetByCategoryShortNameAsync(string shortName)
            => await _context.Voxs
                        .Where(x => x.Category.ShortName == shortName && x.State == VoxState.Normal)
                        .Include(x => x.Media)
                        .Include(x => x.Category)
                        .Include(x => x.Comments)
                        .OrderByDescending(x => x.Bump)
                        .Skip(0)
                        .Take(100)
                        .AsNoTracking()
                        .ToListAsync();
        //agregar as no tracking

        //public async Task<Vox> GetLastVoxBump(IEnumerable<string> hash)
        // => await _context.Voxs
        //    .Where(x => hash.Contains(x.Hash))
        //    .OrderBy(x => x.Bump)
        //    .FirstAsync();

        public async Task<Vox> GetLastVoxBump(IEnumerable<Guid> skipIds)
         => await _context.Voxs
            .Where(x => skipIds.Contains(x.ID))
            .OrderBy(x => x.Bump)
            .AsNoTracking()
            .FirstAsync();
        //agregar as no tracking
    }
}
