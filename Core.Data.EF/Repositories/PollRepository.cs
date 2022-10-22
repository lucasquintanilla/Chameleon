using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class PollRepository : GenericRepository<Poll>, IPollRepository
    {
        public PollRepository(VoxedContext context) : base(context) { }
    }
}
