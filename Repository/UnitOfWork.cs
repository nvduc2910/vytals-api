using Vytals.Models;
using Vytals.Repository.Implementation;
using Vytals.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vytals.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed;
        private readonly VfDbContext context;
        private Dictionary<Type, object> repositories;

        public UnitOfWork(VfDbContext context)
        {
            this.context = context;
        }


        public VfDbContext GetContext()
        {
            return context;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(context);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    repositories?.Clear();

                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
