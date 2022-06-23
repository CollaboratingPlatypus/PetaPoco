using System;
using System.Reflection;

namespace PetaPoco.Core
{
    public class PocoColumn
    {
        public bool AutoSelectedResultColumn;
        public string ColumnName;
        public bool ForceToUtc;
        public bool ForceToAnsiString;
        public bool ForceToDateTime2;
        public PropertyInfo PropertyInfo;
        public bool ResultColumn;
        public string InsertTemplate { get; set; }
        public string UpdateTemplate { get; set; }

        public virtual void SetValue(object target, object val)
        {
            PropertyInfo.SetValue(target, val, null);
        }

        public virtual object GetValue(object target)
        {
            return PropertyInfo.GetValue(target, null);
        }

        public virtual object ChangeType(object val)
        {
            var t = PropertyInfo.PropertyType;
            if (val.GetType().IsValueType && PropertyInfo.PropertyType.IsGenericType && PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                t = t.GetGenericArguments()[0];

            return Convert.ChangeType(val, t);
        }
    }
}