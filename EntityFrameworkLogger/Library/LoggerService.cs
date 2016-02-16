using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using EntityFrameworkLogger.Model;

namespace EntityFrameworkLogger.Library
{
    public class LoggerService
    {
        private readonly EntityFrameworkLoggerContext _context;
        private readonly IDictionary<Type, IList> _loggingDict = new Dictionary<Type, IList>();

        public LoggerService(EntityFrameworkLoggerContext context)
        {
            _context = context;
        }

        public void LogChanges()
        {
            foreach (var dbEntityEntry in _context.ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted))
            {
                switch (dbEntityEntry.State)
                {
                    case EntityState.Added:
                        AddEntityChange(dbEntityEntry, EntityOperations.Added);
                        break;
                    case EntityState.Modified:
                        AddEntityChange(dbEntityEntry, EntityOperations.Modified);
                        break;
                    case EntityState.Deleted:
                        AddEntityChange(dbEntityEntry, EntityOperations.Deleted);
                        break;
                }
            }
        }

        public IReadOnlyCollection<EntityChange<TEntity>> GetChangesByEntityId<TEntity>(int entityId)
        {
            return (IReadOnlyCollection<EntityChange<TEntity>>) _loggingDict[typeof(TEntity)];
        }

        private void AddEntityChange(DbEntityEntry dbEntityEntry, EntityOperations entityOperation)
        {
            var entityType = dbEntityEntry.Entity.GetType().BaseType; // cause we are using dynamic proxies web must use BaseType
            var entityChangeType = typeof(EntityChange<>).MakeGenericType(entityType);
            dynamic entityChange = Activator.CreateInstance(entityChangeType);

            entityChange.EntityOperation = entityOperation;
            entityChange.OperationDateTime = DateTime.Now;
            entityChange.ValueChanges =
                dbEntityEntry.CurrentValues.PropertyNames.Select(
                    propertyName => new EntityValueChange(propertyName, dbEntityEntry.OriginalValues[propertyName], dbEntityEntry.CurrentValues[propertyName])).ToList();

            CreateOrUpdateEntityChange(entityChange, entityType);
        }

        private void CreateOrUpdateEntityChange(object entityChange, Type entityType)
        {
            if (_loggingDict.ContainsKey(entityType) == false)
            {
                var type = typeof(EntityChange<>).MakeGenericType(entityType);
                var listType = typeof(List<>).MakeGenericType(type);
                var list = (IList) Activator.CreateInstance(listType);

                _loggingDict.Add(entityType, list);
            }

            _loggingDict[entityType].Add(entityChange);
        }
    }
}