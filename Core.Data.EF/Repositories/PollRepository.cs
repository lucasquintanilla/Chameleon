using Core.Data.Repositories;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class PollRepository : GenericRepository<Poll>, IPollRepository
    {
        private readonly VoxedContext _context;

        public PollRepository(VoxedContext context) : base(context)
        {
            _context = context;
        }
    }
}
