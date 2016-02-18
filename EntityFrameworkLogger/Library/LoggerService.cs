using System;
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
        private readonly LoggerStorage _loggerStorage;

        public LoggerService(EntityFrameworkLoggerContext context, LoggerStorage loggerStorage)
        {
            _context = context;
            _loggerStorage = loggerStorage;
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

            _loggerStorage.AddEntityChange(entityChange, entityType);
        }
    }
}