using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IUserPostActionRepository : IRepository<UserVoxAction>
    {
        Task<UserVoxAction> GetByUserIdVoxId(Guid userId, Guid voxId);
        Task<IList<Guid>> GetVoxSubscriberUserIds(Guid voxId, List<Guid> ignoreUserIds);
    }
}
