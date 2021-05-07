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
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(VoxedContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> GetByUserId(Guid userId)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .Include(x => x.Comment)
                .Include(x => x.Vox)
                .Include(x => x.Vox.Media)
                .Include(x => x.Owner)
                .ToListAsync();
        }

        public async Task<Notification> GetByVoxId(Guid voxId)
        {
            return await _context.Notifications
                .Where(x => x.VoxId == voxId)
                .FirstOrDefaultAsync();
        }
    }
}
