﻿using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly VoxedContext context;

        public CategoryRepository(VoxedContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Categories.AnyAsync(c => c.ID == id);
        }

        public override async Task<IEnumerable<Category>> GetAll() 
        {
            return await context.Categories
                .OrderBy(c => c.Name)
                .Include(c => c.Media)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Category> GetById(int id)
        {
            return await context.Categories
                .Include(c => c.Media)
                .FirstOrDefaultAsync(c => c.ID == id);
        }
    }
}