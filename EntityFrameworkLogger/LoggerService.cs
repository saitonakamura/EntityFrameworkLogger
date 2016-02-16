using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using EntityFrameworkLogger.Model;

namespace EntityFrameworkLogger
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
                        LogAddedEntity(dbEntityEntry);
                        break;
                    case EntityState.Modified:
                        LogModifiedEntity(dbEntityEntry);
                        break;
                    case EntityState.Deleted:
                        LogDeletedEntity(dbEntityEntry);
                        break;
                }
            }
        }

        private void LogAddedEntity(DbEntityEntry dbEntityEntry)
        {
            var type = typeof(EntityChange<>).MakeGenericType(dbEntityEntry.Entity.GetType());
            dynamic entityChange = Activator.CreateInstance(type);

            entityChange.EntityOperation = EntityOperations.Add;
            entityChange.OperationDateTime = DateTime.Now;
            entityChange.ValueChanges = 
                dbEntityEntry.CurrentValues.PropertyNames.Select(
                    propertyName => new EntityValueChange(propertyName, dbEntityEntry.CurrentValues[propertyName], null)).ToList();

            CreateOrUpdateEntityChange(entityChange, dbEntityEntry.Entity.GetType());
        }

        private void LogModifiedEntity(DbEntityEntry dbEntityEntry)
        {
            var type = typeof(EntityChange<>).MakeGenericType(dbEntityEntry.Entity.GetType());
            dynamic entityChange = Activator.CreateInstance(type);

            entityChange.EntityOperation = EntityOperations.Modified;
            entityChange.OperationDateTime = DateTime.Now;
            entityChange.ValueChanges =
                dbEntityEntry.CurrentValues.PropertyNames.Select(
                    propertyName => new EntityValueChange(propertyName, dbEntityEntry.CurrentValues[propertyName], dbEntityEntry.OriginalValues[propertyName])).ToList();

            CreateOrUpdateEntityChange(entityChange, dbEntityEntry.Entity.GetType());
        }

        private void LogDeletedEntity(DbEntityEntry dbEntityEntry)
        {
            var type = typeof(EntityChange<>).MakeGenericType(dbEntityEntry.Entity.GetType());
            dynamic entityChange = Activator.CreateInstance(type);

            entityChange.EntityOperation = EntityOperations.Deleted;
            entityChange.OperationDateTime = DateTime.Now;
            entityChange.ValueChanges =
                dbEntityEntry.CurrentValues.PropertyNames.Select(
                    propertyName => new EntityValueChange(propertyName, null, dbEntityEntry.OriginalValues[propertyName])).ToList();

            CreateOrUpdateEntityChange(entityChange, dbEntityEntry.Entity.GetType());
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

        public IReadOnlyCollection<EntityChange<TEntity>> GetChangesByEntityId<TEntity>(int entityId)
        {
            return (IReadOnlyCollection<EntityChange<TEntity>>) _loggingDict[typeof(TEntity)];
        }
    }

    public class EntityChange<TEntity>
    {
        public EntityOperations EntityOperation { get; set; }
        public DateTime OperationDateTime { get; set; }
        public IReadOnlyCollection<EntityValueChange> ValueChanges { get; set; }

        public EntityChange()
        {
        }

        public EntityChange(EntityOperations entityOperation, DateTime operationDateTime, IReadOnlyCollection<EntityValueChange> valueChanges)
        {
            EntityOperation = entityOperation;
            OperationDateTime = operationDateTime;
            ValueChanges = valueChanges;
        }
    }

    public class EntityValueChange
    {
        public string FieldName { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public EntityValueChange(string fieldName, object oldValue, object newValue)
        {
            FieldName = fieldName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public enum EntityOperations
    {
        Add,
        Modified,
        Deleted,
    }
}