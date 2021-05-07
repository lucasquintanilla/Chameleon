using System;
using System.Collections.Generic;
using System.Text;
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
        Task<int> CompleteAsync();
    }
}
