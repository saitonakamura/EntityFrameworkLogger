using System;
using System.Collections;
using System.Collections.Generic;

namespace EntityFrameworkLogger.Library
{
    public class LoggerStorage
    {
        private readonly IDictionary<Type, IList> _loggingDict = new Dictionary<Type, IList>();

        public LoggerStorage()
        {
        }

        public void AddEntityChange(object entityChange, Type entityType)
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
            return (IReadOnlyCollection<EntityChange<TEntity>>)_loggingDict[typeof(TEntity)];
        }
    }
}