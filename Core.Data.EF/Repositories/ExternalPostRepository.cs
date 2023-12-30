﻿using Core.Data.Repositories;
using Core.Entities;

namespace Core.Data.EF.Repositories
{
    public class ExternalPostRepository : Repository<Tag>, IExternalPostRepository
    {
        public ExternalPostRepository(VoxedContext context) : base(context) { }
    }
}
