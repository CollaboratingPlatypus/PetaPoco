// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/06/29</date>

using System;
using System.Linq;
using System.Reflection;
using PetaPoco.Core.Inflection;

namespace PetaPoco
{
    /// <summary>
    ///     Represents a configurable convention mapper.
    /// </summary>
    /// <remarks>
    ///     By default this mapper replaces <see cref="StandardMapper" /> without change, which means backwards compatibility
    ///     is kept.
    /// </remarks>
    public class ConventionMapper : IMapper
    {
        /// <summary>
        ///     Gets or sets the get sequence name logic.
        /// </summary>
        public Func<Type, PropertyInfo, string> GetSequenceName { get; set; }

        /// <summary>
        ///     Gets or sets the inflect column name logic.
        /// </summary>
        public Func<IInflector, string, string> InflectColumnName { get; set; }

        /// <summary>
        ///     Gets or sets the inflect table name logic.
        /// </summary>
        public Func<IInflector, string, string> InflectTableName { get; set; }

        /// <summary>
        ///     Gets or sets the is primary key auto increment logic.
        /// </summary>
        public Func<Type, bool> IsPrimaryKeyAutoIncrement { get; set; }

        /// <summary>
        ///     Gets or sets the map column logic.
        /// </summary>
        public Func<ColumnInfo, Type, PropertyInfo, bool> MapColumn { get; set; }

        /// <summary>
        ///     Gets or set the map primary key logic.
        /// </summary>
        public Func<TableInfo, Type, bool> MapPrimaryKey { get; set; }

        /// <summary>
        ///     Gets or sets the map table logic.
        /// </summary>
        public Func<TableInfo, Type, bool> MapTable { get; set; }

        /// <summary>
        ///     Gets or sets the from db convert logic.
        /// </summary>
        public Func<PropertyInfo, Type, Func<object, object>> FromDbConverter { get; set; }

        /// <summary>
        ///     Gets or sets the to db converter logic.
        /// </summary>
        public Func<PropertyInfo, Func<object, object>> ToDbConverter { get; set; }

        /// <summary>
        ///     Constructs a new instance of convention mapper.
        /// </summary>
        public ConventionMapper()
        {
            GetSequenceName = (t, pi) => null;
            InflectColumnName = (inflect, cn) => cn;
            InflectTableName = (inflect, tn) => tn;
            MapPrimaryKey = (ti, t) =>
            {
                var primaryKey = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).FirstOrDefault() as PrimaryKeyAttribute;

                if (primaryKey != null)
                {
                    ti.PrimaryKey = primaryKey.Value;
                    ti.SequenceName = primaryKey.SequenceName;
                    ti.AutoIncrement = primaryKey.AutoIncrement;
                    return true;
                }

                var prop = t.GetProperties().FirstOrDefault(p =>
                {
                    if (p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (p.Name.Equals(t.Name + "Id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (p.Name.Equals(t.Name + "_Id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    return false;
                });

                if (prop == null)
                    return false;

                ti.PrimaryKey = InflectColumnName(Inflector.Instance, prop.Name);
                ti.AutoIncrement = IsPrimaryKeyAutoIncrement(prop.PropertyType);
                ti.SequenceName = GetSequenceName(t, prop);
                return true;
            };
            MapTable = (ti, t) =>
            {
                var tableName = t.GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
                ti.TableName = tableName != null ? tableName.Value : InflectTableName(Inflector.Instance, t.Name);
                MapPrimaryKey(ti, t);
                return true;
            };
            IsPrimaryKeyAutoIncrement = t =>
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    t = t.GetGenericArguments()[0];

                if (t == typeof(long) || t == typeof(ulong))
                    return true;
                if (t == typeof(int) || t == typeof(uint))
                    return true;
                if (t == typeof(short) || t == typeof(ushort))
                    return true;

                return false;
            };
            MapColumn = (ci, t, pi) =>
            {
                // Check if declaring poco has [Explicit] attribute
                var isExplicit = t.GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Any();

                // Check for [Column]/[Ignore] Attributes
                var column = pi.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;

                if (isExplicit && column == null)
                    return false;

                if (pi.GetCustomAttributes(typeof(IgnoreAttribute), true).Any())
                    return false;

                // Read attribute
                if (column != null)
                {
                    ci.ColumnName = column.Name ?? InflectColumnName(Inflector.Instance, pi.Name);
                    ci.ForceToUtc = column.ForceToUtc;
                    ci.ResultColumn = (column as ResultColumnAttribute) != null;
                    ci.InsertTemplate = column.InsertTemplate;
                    ci.UpdateTemplate = column.UpdateTemplate;
                }
                else
                {
                    ci.ColumnName = InflectColumnName(Inflector.Instance, pi.Name);
                }

                return true;
            };
            FromDbConverter = (pi, t) =>
            {
                if (pi != null)
                {
                    var valueConverter = pi.GetCustomAttributes(typeof(ValueConverterAttribute), true).FirstOrDefault() as ValueConverterAttribute;
                    if (valueConverter != null)
                        return valueConverter.ConvertFromDb;
                }

                return null;
            };
            ToDbConverter = (pi) =>
            {
                if (pi != null)
                {
                    var valueConverter = pi.GetCustomAttributes(typeof(ValueConverterAttribute), true).FirstOrDefault() as ValueConverterAttribute;
                    if (valueConverter != null)
                        return valueConverter.ConvertToDb;
                }

                return null;
            };
        }

        /// <summary>
        ///     Get information about the table associated with a POCO class
        /// </summary>
        /// <param name="pocoType">The poco type.</param>
        /// <returns>A TableInfo instance</returns>
        /// <remarks>
        ///     This method must return a valid TableInfo.
        ///     To create a TableInfo from a POCO's attributes, use TableInfo.FromPoco
        /// </remarks>
        public TableInfo GetTableInfo(Type pocoType)
        {
            var ti = new TableInfo();
            return MapTable(ti, pocoType) ? ti : null;
        }

        /// <summary>
        ///     Get information about the column associated with a property of a POCO
        /// </summary>
        /// <param name="pocoProperty">The PropertyInfo of the property being queried</param>
        /// <returns>A reference to a ColumnInfo instance, or null to ignore this property</returns>
        /// <remarks>
        ///     To create a ColumnInfo from a property's attributes, use PropertyInfo.FromProperty
        /// </remarks>
        public ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
        {
            var ci = new ColumnInfo();
            return MapColumn(ci, pocoProperty.DeclaringType, pocoProperty) ? ci : null;
        }

        /// <summary>
        ///     Supply a function to convert a database value to the correct property value
        /// </summary>
        /// <param name="targetProperty">The target property</param>
        /// <param name="sourceType">The type of data returned by the DB</param>
        /// <returns>A Func that can do the conversion, or null for no conversion</returns>
        public Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType)
        {
            return FromDbConverter?.Invoke(targetProperty, sourceType);
        }

        /// <summary>
        ///     Supply a function to convert a property value into a database value
        /// </summary>
        /// <param name="sourceProperty">The property to be converted</param>
        /// <returns>A Func that can do the conversion</returns>
        /// <remarks>
        ///     This conversion is only used for converting values from POCO's that are
        ///     being Inserted or Updated.
        ///     Conversion is not available for parameter values passed directly to queries.
        /// </remarks>
        public Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
        {
            return ToDbConverter?.Invoke(sourceProperty);
        }
    }
}