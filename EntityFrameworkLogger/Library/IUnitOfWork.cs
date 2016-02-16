using System;
using System.Linq;

namespace EntityFrameworkLogger.Library
{
    public interface IUnitOfWork
    {
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;

        void Add<TEntity>(TEntity entity) where TEntity : class;

        void Remove<TEntity>(TEntity entity) where TEntity : class;

        [SaveChanges]
        void SaveChanges();
    }

    public class SaveChangesAttribute : Attribute
    {
    }
}