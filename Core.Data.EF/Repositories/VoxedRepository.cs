﻿using Core.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class VoxedRepository : IVoxedRepository
    {
        private readonly VoxedContext _context;

        public VoxedRepository(VoxedContext context)
        {
            //context.Database.Log = Console.Write; // .NET6 feature
            _context = context;
            Voxs = new VoxRepository(context);
            Categories = new CategoryRepository(context);
            Comments = new CommentRepository(context);
            Media = new MediaRepository(context);
            Polls = new PollRepository(context);
            Notifications = new NotificationRepository(context);
            UserVoxActions = new UserVoxActionRepository(context);
        }

        public IVoxRepository Voxs { get; }
        public ICategoryRepository Categories { get; }
        public IMediaRepository Media { get; }
        public ICommentRepository Comments { get; }
        public IPollRepository Polls { get; }
        public INotificationRepository Notifications { get; }
        public IUserVoxActionRepository UserVoxActions { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
