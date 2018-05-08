using Vytals.Models;
using Vytals.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vytals.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        VfDbContext GetContext();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}
