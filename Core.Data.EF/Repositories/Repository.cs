using Core.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly VoxedContext _context;
        protected DbSet<T> Entities => _context.Set<T>();

        public Repository(VoxedContext context)
        {
            _context = context;
        }

        public virtual async Task Add(T entity)
        {
            await Entities.AddAsync(entity);
        }

        public virtual async Task AddRange(IEnumerable<T> entities)
        {
            await Entities.AddRangeAsync(entities);
        }

        public virtual async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await Entities.Where(expression).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await Entities.ToListAsync();
        }

        public virtual async Task<T> GetById(int id)
        {
            return await Entities.FindAsync(id);
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await Entities.FindAsync(id);
        }

        public virtual async Task Remove(T entity)
        {
            await Task.Run(() => Entities.Remove(entity));
        }

        public virtual async Task RemoveRange(IEnumerable<T> entities)
        {
            await Task.Run(() => _context.Set<T>().RemoveRange(entities));
        }
    }
}
