using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class UserVoxActionRepository : GenericRepository<UserVoxAction>, IUserVoxActionRepository
    {
        private readonly VoxedContext _context;

        public UserVoxActionRepository(VoxedContext context) : base(context)
        {
            _context = context;
        }
    }
}
