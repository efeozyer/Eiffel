using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Persistence.Abstractions
{
    public interface IRepository<in TContext, TEntity, TKey>
        where TContext : DbContext
        where TEntity : Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        TEntity Get(TKey id);

        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Remove(TKey id);

        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
