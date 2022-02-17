using System;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IVoxedRepository : IDisposable
    {
        IVoxRepository Voxs { get; }
        ICategoryRepository Categories { get; }
        IMediaRepository Media { get; }
        ICommentRepository Comments { get; }
        INotificationRepository Notifications { get; }
        Task<int> SaveChangesAsync();
    }
}
