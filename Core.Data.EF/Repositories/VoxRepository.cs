using Core.Data.EF.Extensions;
using Core.Data.Filters;
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
                .Include(x => x.Comments.Where(c => c.State == CommentState.Active)) //agregar order by descending
                    .ThenInclude(c => c.Media)
                .Include(x => x.Comments.Where(c => c.State == CommentState.Active))
                    .ThenInclude(c => c.User)
                .Include(x => x.Poll)
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.ID == id);

        public async Task<List<Vox>> GetByFilterAsync(VoxFilter filter)
        {
            var query = _context.Voxs.AsNoTracking();

            query = query.Where(x => x.State == VoxState.Normal)
                       .Include(x => x.Media)
                       .Include(x => x.Category)
                       .Include(x => x.Comments.Where(c => c.State == CommentState.Active));

            if (filter.UserId.HasValue)
            {
                if (filter.IncludeHidden)
                {
                    query = query.Where(x => _context.UserVoxActions.AsNoTracking().Where(u => u.UserId == filter.UserId.Value && u.IsHidden).Select(v => v.VoxId).Contains(x.ID));
                }
                else
                {
                    query = query.Where(x => !_context.UserVoxActions.AsNoTracking().Where(x => x.UserId == filter.UserId.Value && x.IsHidden).Select(x => x.VoxId).Contains(x.ID));
                }

                if (filter.IncludeFavorites)
                {
                    query = query.Where(x => _context.UserVoxActions.AsNoTracking().Where(u => u.UserId == filter.UserId.Value && u.IsFavorite).Select(v => v.VoxId).Contains(x.ID));
                }
            }

            if (filter.IgnoreVoxIds.Any())
            {
                query = query.Where(x => !filter.IgnoreVoxIds.Contains(x.ID));

                var lastVox = await GetLastVoxBump(filter.IgnoreVoxIds);
                query = query.Where(x => x.Bump < lastVox.Bump);
            }

            if (filter.Categories.Any())
            {
                query = query.Where(x => filter.Categories.Contains(x.CategoryID));
            }

            if (!string.IsNullOrEmpty(filter.Search))
            {
                var keywords = filter.Search.ToLower().Split(' ').Distinct();

                var predicateTitle = keywords.Select(k => (Expression<Func<Vox, bool>>)(x => x.Title.Contains(k))).ToArray();
                //var predicateContent = keywords.Select(k => (Expression<Func<Vox, bool>>)(x => x.Content.Contains(k))).ToArray();      
                query = query.WhereAny(predicateTitle);
            }

            if (filter.HiddenWords.Any())
            {
                var hiddenWords = filter.HiddenWords.Distinct();

                var predicateTitle = hiddenWords.Select(k => (Expression<Func<Vox, bool>>)(x => !x.Title.Contains(k))).ToArray();
                //var predicateContent = keywords.Select(k => (Expression<Func<Vox, bool>>)(x => x.Content.Contains(k))).ToArray();      
                query = query.WhereAny(predicateTitle);
            }

            query = query
                       .OrderByDescending(x => x.IsSticky).ThenByDescending(x => x.Bump)
                       .Skip(0)
                       .Take(36);

            var result = query.ToList();

            return await Task.FromResult(result);
        }

        private async Task<Vox> GetLastVoxBump(IEnumerable<Guid> skipIds)
         => await _context.Voxs
            .Where(x => skipIds.Contains(x.ID))
            .OrderBy(x => x.Bump)
            .AsNoTracking()
            .FirstAsync();
    }
}
