using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class MediaRepository : GenericRepository<Attachment>, IMediaRepository
    {
        public MediaRepository(VoxedContext context) : base(context) { }
    }
}
