using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuthServer.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthServer.Persistence.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public Repository(DbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public TEntity Get(int id)
        {
            return _entities.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _entities.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.SingleOrDefault(predicate);
        }

        public Task<List<TEntity>> ToListAsync()
        {
           return _context.Set<TEntity>().ToListAsync();
        }

        public ValueTask<TEntity> FindAsync(int id)
        {
            return _context.Set<TEntity>().FindAsync(id);
        }

        public void AddAsync(TEntity entity)
        {
            _entities.AddAsync(entity);
        }
        public void Add(TEntity entity)
        {
            _entities.AddAsync(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _entities.RemoveRange(entities);
        }

        public IDbContextTransaction BeginTransaction(){
            return _context.Database.BeginTransaction();
        }

        public void TrackEntities(IEnumerable<TEntity> entities){
            _context.UpdateRange(entities);
        }

        public void TrackEntity(TEntity entity){
            _context.UpdateRange(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}