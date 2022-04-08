using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class UserVoxActionRepository : GenericRepository<UserVoxAction>, IUserVoxActionRepository
    {
        private readonly VoxedContext _context;

        public UserVoxActionRepository(VoxedContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserVoxAction> GetByUserIdVoxId(Guid userId, Guid voxId)
         => await _context.UserVoxActions
            .Where(x => x.UserId == userId && x.VoxId == voxId)            
            .SingleOrDefaultAsync();

        public async Task<IList<Guid>> GetVoxSubscriberUserIds(Guid voxId)
        {
            return await _context.UserVoxActions.Where(x => x.VoxId == voxId && x.IsFollowed).Select(x => x.UserId).ToListAsync();
        }
    }
}
