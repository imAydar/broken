using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BrokenCode.Interfaces
{
    /// <summary>
    /// Might be an over kill, but i have that class developed before, and i think it's good to have
    /// a base class if it's possible. Also although it is an interface and i don't move it to namespace
    /// BrokenService.Interfaces cuz that's part of something more common.
    /// </summary>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Get all entities from the database.
        /// </summary>
        /// <returns>All the entities.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Get an entity from the database.
        /// </summary>
        /// <param name="id">An id of entity.</param>
        /// <returns>Entity.</returns>
        Task<TEntity> GetByIdAsync(int id);

        /// <summary>
        /// Get entities typed as IQueryable.
        /// </summary>
        /// <returns>IQueryable object.</returns>
        IQueryable<TEntity> AsQueryable();

        /// <summary>
        /// Get an entities from the database.
        /// </summary>
        /// <param name="predicate">A predicate to decide which entities to get.</param>
        /// <returns>All entities that matches the predicate.</returns>
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Create an entity in database.
        /// </summary>
        /// <param name="entity">Entity to create.</param>
        /// <returns>Created entity.</returns>
        Task<TEntity> CreateAsync(TEntity entity);

        /// <summary>
        /// Update an entity in database.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>Updated entity.</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Delete an entity in database.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>Deleted entity.</returns>
        Task<TEntity> DeleteAsync(TEntity entity);
    }
}
