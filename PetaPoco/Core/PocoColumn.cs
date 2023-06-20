using System;
using System.Reflection;

namespace PetaPoco.Core
{
    /// <summary>
    /// Represents a property defined in a POCO object which is mapped to a column in that POCO's corresponding table.
    /// </summary>
    public class PocoColumn
    {
        /// <inheritdoc cref="ColumnInfo.ColumnName"/>
        public string ColumnName  { get; set; }

        /// <inheritdoc cref="ColumnInfo.ResultColumn"/>
        public bool ResultColumn  { get; set; }

        /// <inheritdoc cref="ColumnInfo.AutoSelectedResultColumn"/>
        public bool AutoSelectedResultColumn  { get; set; }

        /// <inheritdoc cref="ColumnInfo.ForceToAnsiString"/>
        public bool ForceToAnsiString  { get; set; }

        /// <inheritdoc cref="ColumnInfo.ForceToDateTime2"/>
        public bool ForceToDateTime2  { get; set; }

        /// <inheritdoc cref="ColumnInfo.ForceToUtc"/>
        public bool ForceToUtc  { get; set; }

        /// <inheritdoc cref="ColumnInfo.InsertTemplate"/>
        public string InsertTemplate { get; set; }

        /// <inheritdoc cref="ColumnInfo.UpdateTemplate"/>
        public string UpdateTemplate { get; set; }

        /// <summary>
        /// Gets or sets the property info for the column-mapped property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <inheritdoc cref="PropertyInfo.SetValue(object, object, object[])"/>
        public virtual void SetValue(object target, object val) => PropertyInfo.SetValue(target, val, null);

        /// <inheritdoc cref="PropertyInfo.GetValue(object, object[])"/>
        public virtual object GetValue(object target) => PropertyInfo.GetValue(target, null);

        /// <inheritdoc cref="Convert.ChangeType(object, Type)"/>
        public virtual object ChangeType(object val)
        {
            var t = PropertyInfo.PropertyType;
            if (val.GetType().IsValueType && PropertyInfo.PropertyType.IsGenericType &&
                PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                t = t.GetGenericArguments()[0];
            return Convert.ChangeType(val, t);
        }
    }
}
