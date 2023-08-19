using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using PetaPoco.Internal;

namespace PetaPoco.Core
{
    /// <summary>
    /// Represents the core data structure for PetaPoco's database operations.
    /// </summary>
    public class PocoData
    {
        private static readonly object _converterLock = new object();

        private static Cache<Type, PocoData> _pocoDatas = new Cache<Type, PocoData>();
        private static List<Func<object, object>> _converters = new List<Func<object, object>>();
        private static MethodInfo fnGetValue = typeof(IDataRecord).GetMethod("GetValue", new Type[] { typeof(int) });
        private static MethodInfo fnIsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");
        private static FieldInfo fldConverters = typeof(PocoData).GetField("_converters", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);
        private static MethodInfo fnListGetItem = typeof(List<Func<object, object>>).GetProperty("Item").GetGetMethod();
        private static MethodInfo fnInvoke = typeof(Func<object, object>).GetMethod("Invoke");
        private Cache<Tuple<string, string, int, int>, Delegate> PocoFactories = new Cache<Tuple<string, string, int, int>, Delegate>();

        /// <summary>
        /// Gets or sets the type of the POCO class represented by the PocoData instance.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets the array of all queryable database column names used by auto-select for query operations when <see
        /// cref="IDatabase.EnableAutoSelect"/> is enabled.
        /// </summary>
        /// <remarks>
        /// Column names are returned unescaped. Escaping should be applied based on the configured <see cref="IDatabase.Provider"/> if
        /// accessing this list to construct an SQL query. To access all
        /// <para>Excluded columns include: columns decorated with the <see cref="IgnoreAttribute"/>, unannotated columns in a POCO marked
        /// with the <see cref="ExplicitColumnsAttribute"/>, and any <see cref="ColumnInfo.ResultColumn"/> that has opted out of auto-select
        /// by setting <see cref="ResultColumnAttribute.IncludeInAutoSelect"/> to <see cref="IncludeInAutoSelect.No"/> or through the <see
        /// cref="ColumnInfo.AutoSelectedResultColumn"/> property.</para>
        /// </remarks>
        public string[] QueryColumns { get; private set; }

        /// <summary>
        /// Gets the array of column names used for update operations, excluding result columns and the primary key.
        /// </summary>
        public string[] UpdateColumns // No need to cache as it's not used by PetaPoco internally
            => (from c in Columns
                where !c.Value.ResultColumn && c.Value.ColumnName != TableInfo.PrimaryKey
                select c.Key).ToArray();

        /// <summary>
        /// Gets the metadata about the database table associated with the POCO class.
        /// </summary>
        public TableInfo TableInfo { get; private set; }

        /// <summary>
        /// Gets the dictionary of PocoColumn objects, containing column metadata for the database table mapped to the POCO class.
        /// </summary>
        public Dictionary<string, PocoColumn> Columns { get; private set; }

        /// <summary>
        /// Initializes a new instance of the PocoData class with default values.
        /// </summary>
        public PocoData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PocoData class with the specified type and mapper.
        /// </summary>
        /// <param name="type">The type of the POCO class.</param>
        /// <param name="defaultMapper">The default mapper to use for the POCO type.</param>
        public PocoData(Type type, IMapper defaultMapper)
        {
            Type = type;

            // Get the mapper for this type
            var mapper = Mappers.GetMapper(type, defaultMapper);

            // Get the table info
            TableInfo = mapper.GetTableInfo(type);

            // Work out bound properties
            Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
            foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                ColumnInfo ci = mapper.GetColumnInfo(pi);
                if (ci == null)
                    continue;

                var pc = new PocoColumn();
                pc.PropertyInfo = pi;
                pc.ColumnName = ci.ColumnName;
                pc.ResultColumn = ci.ResultColumn;
                pc.AutoSelectedResultColumn = ci.AutoSelectedResultColumn;
                pc.ForceToUtc = ci.ForceToUtc;
                pc.ForceToAnsiString = ci.ForceToAnsiString;
                pc.ForceToDateTime2 = ci.ForceToDateTime2;
                pc.InsertTemplate = ci.InsertTemplate;
                pc.UpdateTemplate = ci.UpdateTemplate;

                // Store it
                Columns.Add(pc.ColumnName, pc);
            }

            // Build column list for auto-select queries
            QueryColumns = (from c in Columns
                            where !c.Value.ResultColumn || c.Value.AutoSelectedResultColumn
                            select c.Key).ToArray();
        }

        /// <summary>
        /// Creates a new PocoData instance for the specified class type and mapper.
        /// </summary>
        /// <param name="type">The type to create a PocoData instance for.</param>
        /// <param name="defaultMapper">The default mapper to use for the type.</param>
        /// <returns>A new PocoData instance for the specified type.</returns>
        /// <exception cref="InvalidOperationException">Trying to use dynamic types with this method.</exception>
        public static PocoData ForType(Type type, IMapper defaultMapper)
        {
            if (type == typeof(System.Dynamic.ExpandoObject))
                throw new InvalidOperationException("Cannot use dynamic types with this method");

            return _pocoDatas.GetOrAdd(type, () => new PocoData(type, defaultMapper));
        }

        /// <summary>
        /// Creates a new PocoData instance for the specified object, specifically a <see cref="System.Dynamic.ExpandoObject"/>.
        /// </summary>
        /// <param name="obj">The object to create a PocoData instance for.</param>
        /// <param name="primaryKeyName">The name of the primary key for the object.</param>
        /// <param name="defaultMapper">The default mapper to use for the object.</param>
        /// <returns>A new PocoData instance for the specified object.</returns>
        public static PocoData ForObject(object obj, string primaryKeyName, IMapper defaultMapper)
        {
            var t = obj.GetType();
            if (t == typeof(System.Dynamic.ExpandoObject))
            {
                var pd = new PocoData();
                pd.TableInfo = new TableInfo();
                pd.Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
                pd.Columns.Add(primaryKeyName, new ExpandoColumn() { ColumnName = primaryKeyName });
                pd.TableInfo.PrimaryKey = primaryKeyName;
                pd.TableInfo.AutoIncrement = true;
                foreach (var col in (obj as IDictionary<string, object>).Keys)
                {
                    if (col != primaryKeyName)
                        pd.Columns.Add(col, new ExpandoColumn() { ColumnName = col });
                }

                return pd;
            }

            return ForType(t, defaultMapper);
        }

        /// <summary>
        /// Creates a factory function to generate and cache a POCO from a data reader record at runtime. Subsequent reads attempt to locate
        /// the object in the <see cref="Cache{TKey, TValue}"/> for performance gains.
        /// </summary>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="firstColumn">The index of the first column in the record's database table.</param>
        /// <param name="columnCount">The number of columns in the record's database table.</param>
        /// <param name="reader">The data reader instance.</param>
        /// <param name="defaultMapper">The default mapper to use for the POCO.</param>
        /// <returns>A delegate that can convert an <see cref="IDataReader"/> record into a POCO.</returns>
        /// <exception cref="InvalidOperationException">The POCO type is a value type, or the POCO type has no default constructor, or the
        /// POCO type is an interface or abstract class.</exception>
        public Delegate GetFactory(string sql, string connectionString, int firstColumn, int columnCount, IDataReader reader, IMapper defaultMapper)
        {
            // Create key for cache lookup
            var key = Tuple.Create(sql, connectionString, firstColumn, columnCount);

            // Check cache
            return PocoFactories.GetOrAdd(key, () =>
            {
                // Create the method
                var m = new DynamicMethod("petapoco_factory_" + PocoFactories.Count.ToString(), returnType: Type, new Type[] { typeof(IDataReader) }, true);
                var il = m.GetILGenerator();
                var mapper = Mappers.GetMapper(Type, defaultMapper);

                if (Type == typeof(object))
                {
                    // var poco = new T()
                    il.Emit(OpCodes.Newobj, typeof(System.Dynamic.ExpandoObject).GetConstructor(Type.EmptyTypes)); // obj

                    MethodInfo fnAdd = typeof(IDictionary<string, object>).GetMethod("Add");

                    // Enumerate all fields generating a set assignment for the column
                    for (int i = firstColumn; i < firstColumn + columnCount; i++)
                    {
                        var srcType = reader.GetFieldType(i);

                        il.Emit(OpCodes.Dup); // obj, obj
                        il.Emit(OpCodes.Ldstr, reader.GetName(i)); // obj, obj, fieldname

                        // Get the converter
                        Func<object, object> converter = mapper.GetFromDbConverter((PropertyInfo)null, srcType);
                        // TODO: No null check for converter?

                        // if (ForceDateTimesToUtc && converter == null && srcType == typeof(DateTime))
                        //     converter = delegate(object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };

                        // Setup stack for call to converter
                        AddConverterToStack(il, converter);

                        // r[i]
                        il.Emit(OpCodes.Ldarg_0); // obj, obj, fieldname, converter?,    rdr
                        il.Emit(OpCodes.Ldc_I4, i); // obj, obj, fieldname, converter?,  rdr,i
                        il.Emit(OpCodes.Callvirt, fnGetValue); // obj, obj, fieldname, converter?,  value

                        // Convert DBNull to null
                        il.Emit(OpCodes.Dup); // obj, obj, fieldname, converter?,  value, value
                        il.Emit(OpCodes.Isinst, typeof(DBNull)); // obj, obj, fieldname, converter?,  value, (value or null)
                        var lblNotNull = il.DefineLabel();
                        il.Emit(OpCodes.Brfalse_S, lblNotNull); // obj, obj, fieldname, converter?,  value
                        il.Emit(OpCodes.Pop); // obj, obj, fieldname, converter?
                        if (converter != null)
                            il.Emit(OpCodes.Pop); // obj, obj, fieldname,
                        il.Emit(OpCodes.Ldnull); // obj, obj, fieldname, null
                        if (converter != null)
                        {
                            var lblReady = il.DefineLabel();
                            il.Emit(OpCodes.Br_S, lblReady);
                            il.MarkLabel(lblNotNull);
                            il.Emit(OpCodes.Callvirt, fnInvoke);
                            il.MarkLabel(lblReady);
                        }
                        else
                        {
                            il.MarkLabel(lblNotNull);
                        }

                        il.Emit(OpCodes.Callvirt, fnAdd);
                    }
                }
                else if (Type.IsValueType || Type == typeof(string) || Type == typeof(byte[]))
                {
                    // Do we need to install a converter?
                    var srcType = reader.GetFieldType(0);
                    var converter = GetConverter(mapper, null, srcType, Type);

                    // "if (!rdr.IsDBNull(i))"
                    il.Emit(OpCodes.Ldarg_0); // rdr
                    il.Emit(OpCodes.Ldc_I4_0); // rdr,0
                    il.Emit(OpCodes.Callvirt, fnIsDBNull); // bool
                    var lblCont = il.DefineLabel();
                    il.Emit(OpCodes.Brfalse_S, lblCont);
                    il.Emit(OpCodes.Ldnull); // null
                    var lblFin = il.DefineLabel();
                    il.Emit(OpCodes.Br_S, lblFin);

                    il.MarkLabel(lblCont);

                    // Setup stack for call to converter
                    AddConverterToStack(il, converter);

                    il.Emit(OpCodes.Ldarg_0); // rdr
                    il.Emit(OpCodes.Ldc_I4_0); // rdr,0
                    il.Emit(OpCodes.Callvirt, fnGetValue); // value

                    // Call the converter
                    if (converter != null)
                        il.Emit(OpCodes.Callvirt, fnInvoke);

                    il.MarkLabel(lblFin);
                    il.Emit(OpCodes.Unbox_Any, Type); // value converted
                }
                else
                {
                    // var poco = new T()
                    var ctor = Type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
                    if (ctor == null)
                        throw new InvalidOperationException("Type [" + Type.FullName + "] should have a default public or non-public constructor");

                    il.Emit(OpCodes.Newobj, ctor);

                    // Enumerate all fields generating a `Set` assignment for the column
                    for (int i = firstColumn; i < firstColumn + columnCount; i++)
                    {
                        // Get the PocoColumn for this db column, ignore if not known
                        PocoColumn pc;
                        if (!Columns.TryGetValue(reader.GetName(i), out pc))
                            continue;

                        // Get the source type for this column
                        var srcType = reader.GetFieldType(i);
                        var dstType = pc.PropertyInfo.PropertyType;

                        // "if (!rdr.IsDBNull(i))"
                        il.Emit(OpCodes.Ldarg_0); // poco,rdr
                        il.Emit(OpCodes.Ldc_I4, i); // poco,rdr,i
                        il.Emit(OpCodes.Callvirt, fnIsDBNull); // poco,bool
                        var lblNext = il.DefineLabel();
                        il.Emit(OpCodes.Brtrue_S, lblNext); // poco

                        il.Emit(OpCodes.Dup); // poco,poco

                        // Do we need to install a converter?
                        var converter = GetConverter(mapper, pc, srcType, dstType);

                        // Fast
                        bool Handled = false;
                        if (converter == null)
                        {
                            var valuegetter = typeof(IDataRecord).GetMethod("Get" + srcType.Name, new Type[] { typeof(int) });
                            if (valuegetter != null && valuegetter.ReturnType == srcType &&
                                (valuegetter.ReturnType == dstType || valuegetter.ReturnType == Nullable.GetUnderlyingType(dstType)))
                            {
                                var valuesetter = pc.PropertyInfo.GetSetMethod(true);
                                if (valuesetter == null)
                                    throw new InvalidOperationException(pc.PropertyInfo.Name + " is either missing a Set method, or the Set method is readonly. If this is intentional, decorate the property with the `PetaPoco.IgnoreAttribute`.");

                                il.Emit(OpCodes.Ldarg_0); // *,rdr
                                il.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                                il.Emit(OpCodes.Callvirt, valuegetter); // *,value

                                // Convert to Nullable
                                if (Nullable.GetUnderlyingType(dstType) != null)
                                {
                                    il.Emit(OpCodes.Newobj, dstType.GetConstructor(new Type[] { Nullable.GetUnderlyingType(dstType) }));
                                }

                                il.Emit(OpCodes.Callvirt, valuesetter); // poco
                                Handled = true;
                            }
                        }

                        // Not so fast
                        if (!Handled)
                        {
                            // Setup stack for call to converter
                            AddConverterToStack(il, converter);

                            // "value = rdr.GetValue(i)"
                            il.Emit(OpCodes.Ldarg_0); // *,rdr
                            il.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                            il.Emit(OpCodes.Callvirt, fnGetValue); // *,value

                            // Call the converter
                            if (converter != null)
                                il.Emit(OpCodes.Callvirt, fnInvoke);

                            // Assign it
                            il.Emit(OpCodes.Unbox_Any, pc.PropertyInfo.PropertyType); // poco,poco,value
                            il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true)); // poco
                        }

                        il.MarkLabel(lblNext);
                    }

                    var fnOnLoaded = RecurseInheritedTypes<MethodInfo>(Type,
                        (x) => x.GetMethod("OnLoaded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null));
                    if (fnOnLoaded != null)
                    {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Callvirt, fnOnLoaded);
                    }
                }

                il.Emit(OpCodes.Ret);

                // Cache it, return it
                return m.CreateDelegate(Expression.GetFuncType(typeof(IDataReader), Type));
            });
        }

        private static void AddConverterToStack(ILGenerator il, Func<object, object> converter)
        {
            if (converter != null)
            {
                // Add the converter
                int converterIndex;

                lock (_converterLock)
                {
                    converterIndex = _converters.Count;
                    _converters.Add(converter);
                }

                // Generate IL to push the converter onto the stack
                il.Emit(OpCodes.Ldsfld, fldConverters);
                il.Emit(OpCodes.Ldc_I4, converterIndex);
                il.Emit(OpCodes.Callvirt, fnListGetItem); // Converter
            }
        }

        private static Func<object, object> GetConverter(IMapper mapper, PocoColumn pc, Type srcType, Type dstType)
        {
            Func<object, object> converter = null;

            // Get converter from the mapper
            if (pc != null)
            {
                converter = mapper.GetFromDbConverter(pc.PropertyInfo, srcType);
                if (converter != null)
                    return converter;
            }

            // Standard DateTime->Utc mapper
            if (pc != null && pc.ForceToUtc && srcType == typeof(DateTime) && (dstType == typeof(DateTime) || dstType == typeof(DateTime?)))
            {
                return delegate (object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };
            }

            // unwrap nullable types
            Type underlyingDstType = Nullable.GetUnderlyingType(dstType);
            if (underlyingDstType != null)
            {
                dstType = underlyingDstType;
            }

            // Forced type conversion including integral types -> enum
            if (dstType.IsEnum && IsIntegralType(srcType))
            {
                var backingDstType = Enum.GetUnderlyingType(dstType);
                if (underlyingDstType != null)
                {
                    // if dstType is Nullable<Enum>, convert to enum value
                    return delegate (object src) { return Enum.ToObject(dstType, src); };
                }
                else if (srcType != backingDstType)
                {
                    return delegate (object src) { return Convert.ChangeType(src, backingDstType, null); };
                }
            }
            else if (!dstType.IsAssignableFrom(srcType))
            {
                if (dstType.IsEnum && srcType == typeof(string))
                {
                    return delegate (object src) { return EnumMapper.EnumFromString(dstType, (string)src); };
                }

                if (dstType == typeof(Guid) && srcType == typeof(string))
                {
                    return delegate (object src) { return Guid.Parse((string)src); };
                }

                if (dstType == typeof(string) && srcType == typeof(Guid))
                {
                    return delegate (object src) { return Convert.ToString(src); };
                }

                return delegate (object src) { return Convert.ChangeType(src, dstType, null); };
            }

            return null;
        }

        private static bool IsIntegralType(Type type)
        {
            var tc = Type.GetTypeCode(type);
            return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
        }

        private static T RecurseInheritedTypes<T>(Type t, Func<Type, T> cb)
        {
            while (t != null)
            {
                T info = cb(t);
                if (info != null)
                    return info;
                t = t.BaseType;
            }

            return default(T);
        }

        /// <summary>
        /// Clears all cached PocoData instances.
        /// </summary>
        /// <remarks>
        /// Call if you have modified a POCO class and need to reset PetaPoco's internal cache.
        /// </remarks>
        public static void FlushCaches()
            => _pocoDatas.Flush();

        /// <summary>
        /// Gets the column name that is mapped to the given property name using a string comparison.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The column name that maps to the given property name.</returns>
        /// <exception cref="ArgumentNullException">No mapped column exists for <paramref name="propertyName"/>.</exception>
        public string GetColumnName(string propertyName)
            => Columns.Values.First(c => c.PropertyInfo.Name.Equals(propertyName)).ColumnName;
    }
}
