using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly VoxedContext _context;

        public CommentRepository(VoxedContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Comment>> GetAll()
            => await _context.Comments
                   .Include(x => x.Media)
                    .Include(x => x.User)
                   .ToListAsync();
    }
}
