using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class MediaRepository : GenericRepository<Media>, IMediaRepository
    {
        private readonly VoxedContext _context;
        public MediaRepository(VoxedContext context) : base(context)
        {
            _context = context;
        }
    }
}
