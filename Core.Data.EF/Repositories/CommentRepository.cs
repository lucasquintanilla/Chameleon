﻿using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(BlogContext context) : base(context) { }

        public override async Task<IEnumerable<Comment>> GetAll()
            => await Entities
                   .Include(x => x.Media)
                   .Include(x => x.Owner)
                   .ToListAsync();

        public async Task<Comment> GetByHash(string hash) 
            => await Entities.Where(x => x.Hash == hash).SingleOrDefaultAsync();

        public async Task<IEnumerable<Guid>> GetUsersByCommentHash(IEnumerable<string> hashList, ICollection<Guid> skipUserId)
            => await Entities
                .Where(x => hashList.Contains(x.Hash) && !skipUserId.Contains(x.UserId))
                .Select(x => x.UserId)
                .ToListAsync();

        //context.Counties.Where(x => EF.Functions.Like(x.Name, $"%{keyword}%")).ToList();

    }
}
