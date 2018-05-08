using Vytals.Exceptions;
using Vytals.Extensions;
using Vytals.Models;
using Vytals.Repository.Interface;
using Vytals.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Vytals.Repository.Implementation
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly VfDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(VfDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            await DeleteAsync(entityToDelete);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
            if (!(await Context.SaveChangesAsync() > 0))
                throw new CrudFailedException(CRUDErrorMessages.DeleteFailed + Separators.Colon + entity.GetType().Name);
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return  orderBy(query).ToList();
            }
            return query.ToList();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            if (!(await Context.SaveChangesAsync() > 0))
                throw new CrudFailedException(CRUDErrorMessages.InsertFailed + Separators.Colon + entity.GetType().Name);

            return entity;
        }

        public async Task InsertAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
            if (!(await Context.SaveChangesAsync() > 0))
                throw new CrudFailedException(CRUDErrorMessages.InsertFailed + Separators.Colon + entities.GetType().Name);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            if (!(await Context.SaveChangesAsync() > 0))
                throw new CrudFailedException(CRUDErrorMessages.UpdateFailed + Separators.Colon + entity.GetType().Name);
        }

        public async Task DeleteAsync(TEntity[] entities)
        {
            DbSet.RemoveRange(entities);
            if (!(await Context.SaveChangesAsync() > 0))
                throw new CrudFailedException(CRUDErrorMessages.DeleteFailed + Separators.Colon + entities.GetType().Name);
        }

        public async Task<bool> CheckAvailableAsync(object id)
        {
            return await DbSet.FindAsync(id) != null;
        }

        public async Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> filter = null, string[] includeProperties = null)
        {
            return await DbSet.IncludeProperties(includeProperties).SingleAsync(filter);
        }

        public  int CountItem()
        {
            return DbSet.Count<TEntity>();
        }
    }
}
