using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class MediaRepository : Repository<Attachment>, IMediaRepository
    {
        public MediaRepository(VoxedContext context) : base(context) { }
    }
}
