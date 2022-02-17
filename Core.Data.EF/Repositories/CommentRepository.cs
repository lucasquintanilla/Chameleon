﻿using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(VoxedContext context) : base(context) { }

        public override async Task<IEnumerable<Comment>> GetAll()
            => await _context.Comments
                   .Include(x => x.Media)
                   .Include(x => x.User)
                   .ToListAsync();

        public async Task<IEnumerable<Guid>> GetUsersByCommentHash(IEnumerable<string> hashList, ICollection<Guid> skipUserId)
            => await _context.Comments
                .Where(x => hashList.Contains(x.Hash) && !skipUserId.Contains(x.UserID))
                .Select(x => x.UserID)
                .ToListAsync();

        //context.Counties.Where(x => EF.Functions.Like(x.Name, $"%{keyword}%")).ToList();

    }
}
