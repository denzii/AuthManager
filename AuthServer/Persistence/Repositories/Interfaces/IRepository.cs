using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthServer.Persistence.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> ToListAsync();

        ValueTask<TEntity> FindAsync(int id);

        void Add(TEntity entity);

        void AddAsync(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        IDbContextTransaction BeginTransaction();

        void Commit(IDbContextTransaction transaction);

        void TrackEntities(IEnumerable<TEntity> entities);
        void TrackEntity(TEntity entity);
    }
}