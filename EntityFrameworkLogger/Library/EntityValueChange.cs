namespace EntityFrameworkLogger.Library
{
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
}