﻿using Application.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class EfBaseRepository<TEntity, TId, TContext> : IBaseRepository<TEntity, TId>, IAsyncRepository<TEntity,TId>
        where TEntity : BaseEntity<TId>, new()
        where TContext : DbContext
    {
        protected readonly TContext Context;

        public EfBaseRepository(TContext context)
        {
            Context = context;
        }

        public TEntity Add(TEntity entity)
        {
            entity.CreatedDate = DateTime.Now;
            Context.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken=default)
        {
            entity.CreatedDate = DateTime.Now;
            await Context.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public void Delete(TEntity entity)
        {
            entity.DeletedDate = DateTime.Now;
            // Hard Delete - Soft Delete
            // Context.Remove(entity); // Hard Delete
            Context.Update(entity);
            Context.SaveChanges();
        }

        public List<TEntity> GetAll()
        {
            return Context.Set<TEntity>().Where(entity=>!entity.DeletedDate.HasValue).ToList();
        }

        public async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Context.Set<TEntity>();

            if (include != null)
                queryable = include(queryable);

            if (predicate != null)
                queryable = queryable.Where(predicate);
           

            return await queryable.ToListAsync();
        }

        public async Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            CancellationToken cancellationToken = default
            )
        {
            IQueryable<TEntity> queryable = Context.Set<TEntity>();
            if (include != null)
                queryable = include(queryable);
            return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public TEntity? GetById(TId id)
        {
            return Context.Set<TEntity>().FirstOrDefault(entity => entity.Id.Equals(id) && !entity.DeletedDate.HasValue);
        }

        public void Update(TEntity entity)
        {
            entity.UpdatedDate = DateTime.Now;
            Context.Update(entity);
            Context.SaveChanges();
        }
    }
}
