using System;
using System.Collections.Generic;

namespace EntityFrameworkLogger.Library
{
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
}