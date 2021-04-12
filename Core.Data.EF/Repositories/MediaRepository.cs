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
    public class MediaRepository : GenericRepository<Media>, IMediaRepository
    {
        private readonly VoxedContext _context;
        public MediaRepository(VoxedContext context) : base(context)
        {
            _context = context;
        }
    }
}
