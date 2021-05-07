using Core.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class VoxedRepository : IVoxedRepository
    {
        private readonly VoxedContext _context;
        public VoxedRepository(VoxedContext context)
        {
            _context = context;
            Voxs = new VoxRepository(context);
            Categories = new CategoryRepository(context);
            Comments = new CommentRepository(context);
            Media = new MediaRepository(context);
            Polls = new PollRepository(context);
            Notifications = new NotificationRepository(context);
        }

        public IVoxRepository Voxs { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IMediaRepository Media { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public IPollRepository Polls { get; private set; }
        public INotificationRepository Notifications { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
