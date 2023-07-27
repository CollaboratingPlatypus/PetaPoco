using System;
using System.Linq;
using System.Reflection;
using PetaPoco.Core.Inflection;

namespace PetaPoco
{
    /// <summary>
    /// The ConventionMapper class represents a configurable convention mapper.
    /// </summary>
    /// <remarks>
    /// By default this mapper replaces the original <see cref="StandardMapper" /> without change, ensuring backwards compatibility.
    /// </remarks>
    public class ConventionMapper : IMapper
    {
        /// <summary>
        /// Gets or sets the sequence name logic (for Oracle).
        /// </summary>
        public Func<Type, PropertyInfo, string> GetSequenceName { get; set; }

        /// <summary>
        /// Gets or sets the inflected column name logic.
        /// </summary>
        public Func<IInflector, string, string> InflectColumnName { get; set; }

        /// <summary>
        /// Gets or sets the inflected table name logic.
        /// </summary>
        public Func<IInflector, string, string> InflectTableName { get; set; }

        /// <summary>
        /// Gets or sets the primary key auto-increment logic.
        /// </summary>
        public Func<Type, bool> IsPrimaryKeyAutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets the map column logic.
        /// </summary>
        public Func<ColumnInfo, Type, PropertyInfo, bool> MapColumn { get; set; }

        /// <summary>
        /// Gets or sets the map primary key logic.
        /// </summary>
        public Func<TableInfo, Type, bool> MapPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the map table logic.
        /// </summary>
        public Func<TableInfo, Type, bool> MapTable { get; set; }

        /// <summary>
        /// Gets or sets the from db convert logic.
        /// </summary>
        public Func<PropertyInfo, Type, Func<object, object>> FromDbConverter { get; set; }

        /// <summary>
        /// Gets or sets the to db converter logic.
        /// </summary>
        public Func<PropertyInfo, Func<object, object>> ToDbConverter { get; set; }

        /// <summary>
        /// Constructs a new instance of convention mapper.
        /// </summary>
        public ConventionMapper()
        {
            GetSequenceName = (t, pi) => null;
            InflectColumnName = (inflect, cn) => cn;
            InflectTableName = (inflect, tn) => tn;
            MapPrimaryKey = (ti, t) =>
            {
                TableInfo.PopulatePrimaryKeyFromPoco(t, ref ti, out var pkAttr, out var idProp);

                if (pkAttr == null && idProp == null)
                    return false;
                else
                {
                    // If there's no pkAttr, then there's extra processing
                    if (pkAttr == null)
                    {
                        ti.PrimaryKey = InflectColumnName(Inflector.Instance, idProp.Name);
                        ti.AutoIncrement = IsPrimaryKeyAutoIncrement(idProp.PropertyType);
                        ti.SequenceName = GetSequenceName(t, idProp);
                    }

                    return true;
                }
            };
            MapTable = (ti, t) =>
            {
                TableInfo.PopulateTableNameFromPoco(t, ref ti, out var tblAttr);
                if (tblAttr == null)
                    ti.TableName = InflectTableName(Inflector.Instance, t.Name);

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
                ColumnInfo.PopulateFromProperty(pi, ref ci, out var columnAttr);

                if (ci == null)
                    return false;
                else
                {
                    // If there's no colAttr.Name, then we got the name
                    // from pi, so inflect it
                    if (columnAttr?.Name == null)
                        ci.ColumnName = InflectColumnName(Inflector.Instance, pi.Name);

                    return true;
                }
            };
            FromDbConverter = (pi, t) =>
            {
                if (pi != null)
                {
                    var valueConverter = Attribute.GetCustomAttributes(pi, typeof(ValueConverterAttribute)).FirstOrDefault() as ValueConverterAttribute;
                    if (valueConverter != null)
                        return valueConverter.ConvertFromDb;
                }

                return null;
            };
            ToDbConverter = (pi) =>
            {
                if (pi != null)
                {
                    var valueConverter = Attribute.GetCustomAttributes(pi, typeof(ValueConverterAttribute)).FirstOrDefault() as ValueConverterAttribute;
                    if (valueConverter != null)
                        return valueConverter.ConvertToDb;
                }

                return null;
            };
        }

        /// <inheritdoc/>
        public virtual TableInfo GetTableInfo(Type pocoType)
        {
            var ti = new TableInfo();
            return MapTable(ti, pocoType) ? ti : null;
        }

        /// <inheritdoc/>
        public virtual ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
        {
            var ci = new ColumnInfo();
            return MapColumn(ci, pocoProperty.DeclaringType, pocoProperty) ? ci : null;
        }

        /// <inheritdoc/>
        public virtual Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType)
        {
            return FromDbConverter?.Invoke(targetProperty, sourceType);
        }

        /// <inheritdoc/>
        public virtual Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
        {
            return ToDbConverter?.Invoke(sourceProperty);
        }
    }
}
