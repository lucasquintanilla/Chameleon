using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class MediaRepository : Repository<Media>, IMediaRepository
    {
        public MediaRepository(BlogContext context) : base(context) { }
    }
}
