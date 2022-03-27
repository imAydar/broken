using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BrokenCode.Data.Repositories
{
    public class BaseRepository<TEntity>: IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// <see cref="UserDbContext"/>
        /// </summary>
        private readonly UserDbContext _context;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="context"></param>
        public BaseRepository(UserDbContext context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        ///<inheritdoc/>
        public IQueryable<TEntity> AsQueryable()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        ///<inheritdoc/>
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        ///<inheritdoc/>
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        ///<inheritdoc/>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        ///<inheritdoc/>
        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = _context.Set<TEntity>().Where(predicate);
            return await entities.ToListAsync();
        }
    }
}