using System.Linq;
using EntityFrameworkLogger.Model;

namespace EntityFrameworkLogger.Library
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EntityFrameworkLoggerContext _context;

        public UnitOfWork(EntityFrameworkLoggerContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Remove(entity);
        }

        [SaveChanges]
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}