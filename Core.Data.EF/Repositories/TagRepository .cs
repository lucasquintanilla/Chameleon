using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(VoxedContext context) : base(context) { }
    }
}
