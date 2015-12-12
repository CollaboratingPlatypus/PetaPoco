// <copyright file="PocoData.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace PetaPoco.Internal
{
    internal class PocoData
    {
        private static Cache<Type, PocoData> _pocoDatas = new Cache<Type, PocoData>();
        private static List<Func<object, object>> _converters = new List<Func<object, object>>();
        private static MethodInfo fnGetValue = typeof(IDataRecord).GetMethod("GetValue", new Type[] {typeof(int)});
        private static MethodInfo fnIsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");
        private static FieldInfo fldConverters = typeof(PocoData).GetField("_converters", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);
        private static MethodInfo fnListGetItem = typeof(List<Func<object, object>>).GetProperty("Item").GetGetMethod();
        private static MethodInfo fnInvoke = typeof(Func<object, object>).GetMethod("Invoke");
        private Cache<Tuple<string, string, int, int>, Delegate> PocoFactories = new Cache<Tuple<string, string, int, int>, Delegate>();
        public Type type;
        public string[] QueryColumns { get; private set; }
        public TableInfo TableInfo { get; private set; }
        public Dictionary<string, PocoColumn> Columns { get; private set; }

        public PocoData()
        {
        }

        public PocoData(Type t)
        {
            type = t;

            // Get the mapper for this type
            var mapper = Mappers.GetMapper(t);

            // Get the table info
            TableInfo = mapper.GetTableInfo(t);

            // Work out bound properties
            Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
            foreach (var pi in t.GetProperties())
            {
                ColumnInfo ci = mapper.GetColumnInfo(pi);
                if (ci == null)
                    continue;

                var pc = new PocoColumn();
                pc.PropertyInfo = pi;
                pc.ColumnName = ci.ColumnName;
                pc.ResultColumn = ci.ResultColumn;
                pc.ForceToUtc = ci.ForceToUtc;

                // Store it
                Columns.Add(pc.ColumnName, pc);
            }

            // Build column list for automatic select
            QueryColumns = (from c in Columns where !c.Value.ResultColumn select c.Key).ToArray();
        }

        public static PocoData ForObject(object o, string primaryKeyName)
        {
            var t = o.GetType();
            if (t == typeof(System.Dynamic.ExpandoObject))
            {
                var pd = new PocoData();
                pd.TableInfo = new TableInfo();
                pd.Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
                pd.Columns.Add(primaryKeyName, new ExpandoColumn() {ColumnName = primaryKeyName});
                pd.TableInfo.PrimaryKey = primaryKeyName;
                pd.TableInfo.AutoIncrement = true;
                foreach (var col in (o as IDictionary<string, object>).Keys)
                {
                    if (col != primaryKeyName)
                        pd.Columns.Add(col, new ExpandoColumn() {ColumnName = col});
                }
                return pd;
            }
            return ForType(t);
        }

        public static PocoData ForType(Type t)
        {
            if (t == typeof(System.Dynamic.ExpandoObject))
                throw new InvalidOperationException("Can't use dynamic types with this method");

            return _pocoDatas.Get(t, () => new PocoData(t));
        }

        private static bool IsIntegralType(Type t)
        {
            var tc = Type.GetTypeCode(t);
            return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
        }

        // Create factory function that can convert a IDataReader record into a POCO
        public Delegate GetFactory(string sql, string connString, int firstColumn, int countColumns, IDataReader r)
        {
            // Check cache
            var key = Tuple.Create<string, string, int, int>(sql, connString, firstColumn, countColumns);

            return PocoFactories.Get(key, () =>
            {
                // Create the method
                var m = new DynamicMethod("petapoco_factory_" + PocoFactories.Count.ToString(), type, new Type[] {typeof(IDataReader)}, true);
                var il = m.GetILGenerator();
                var mapper = Mappers.GetMapper(type);

                if (type == typeof(object))
                {
                    // var poco=new T()
                    il.Emit(OpCodes.Newobj, typeof(System.Dynamic.ExpandoObject).GetConstructor(Type.EmptyTypes)); // obj

                    MethodInfo fnAdd = typeof(IDictionary<string, object>).GetMethod("Add");

                    // Enumerate all fields generating a set assignment for the column
                    for (int i = firstColumn; i < firstColumn + countColumns; i++)
                    {
                        var srcType = r.GetFieldType(i);

                        il.Emit(OpCodes.Dup); // obj, obj
                        il.Emit(OpCodes.Ldstr, r.GetName(i)); // obj, obj, fieldname

                        // Get the converter
                        Func<object, object> converter = mapper.GetFromDbConverter((PropertyInfo) null, srcType);

                        /*
						if (ForceDateTimesToUtc && converter == null && srcType == typeof(DateTime))
							converter = delegate(object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };
						 */

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
                else
                    if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                    {
                        // Do we need to install a converter?
                        var srcType = r.GetFieldType(0);
                        var converter = GetConverter(mapper, null, srcType, type);

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
                        il.Emit(OpCodes.Unbox_Any, type); // value converted
                    }
                    else
                    {
                        // var poco=new T()
                        il.Emit(OpCodes.Newobj,
                            type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null));

                        // Enumerate all fields generating a set assignment for the column
                        for (int i = firstColumn; i < firstColumn + countColumns; i++)
                        {
                            // Get the PocoColumn for this db column, ignore if not known
                            PocoColumn pc;
                            if (!Columns.TryGetValue(r.GetName(i), out pc))
                                continue;

                            // Get the source type for this column
                            var srcType = r.GetFieldType(i);
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
                                var valuegetter = typeof(IDataRecord).GetMethod("Get" + srcType.Name, new Type[] {typeof(int)});
                                if (valuegetter != null
                                    && valuegetter.ReturnType == srcType
                                    && (valuegetter.ReturnType == dstType || valuegetter.ReturnType == Nullable.GetUnderlyingType(dstType)))
                                {
                                    il.Emit(OpCodes.Ldarg_0); // *,rdr
                                    il.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                                    il.Emit(OpCodes.Callvirt, valuegetter); // *,value

                                    // Convert to Nullable
                                    if (Nullable.GetUnderlyingType(dstType) != null)
                                    {
                                        il.Emit(OpCodes.Newobj, dstType.GetConstructor(new Type[] {Nullable.GetUnderlyingType(dstType)}));
                                    }

                                    il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true)); // poco
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

                        var fnOnLoaded = RecurseInheritedTypes<MethodInfo>(type,
                            (x) => x.GetMethod("OnLoaded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null));
                        if (fnOnLoaded != null)
                        {
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Callvirt, fnOnLoaded);
                        }
                    }

                il.Emit(OpCodes.Ret);

                // Cache it, return it
                return m.CreateDelegate(Expression.GetFuncType(typeof(IDataReader), type));
            }
                );
        }

        private static void AddConverterToStack(ILGenerator il, Func<object, object> converter)
        {
            if (converter != null)
            {
                // Add the converter
                int converterIndex = _converters.Count;
                _converters.Add(converter);

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
                return delegate(object src) { return new DateTime(((DateTime) src).Ticks, DateTimeKind.Utc); };
            }

            // Forced type conversion including integral types -> enum
            if (dstType.IsEnum && IsIntegralType(srcType))
            {
                if (srcType != typeof(int))
                {
                    return delegate(object src) { return Convert.ChangeType(src, typeof(int), null); };
                }
            }
            else if (!dstType.IsAssignableFrom(srcType))
            {
                if (dstType.IsEnum && srcType == typeof(string))
                {
                    return delegate(object src) { return EnumMapper.EnumFromString(dstType, (string) src); };
                }
                else if (dstType == typeof(Guid) && srcType == typeof(string))
                {
                    return delegate (object src) { return Guid.Parse((string)src); };
                }
                else
                {
                    return delegate(object src) { return Convert.ChangeType(src, dstType, null); };
                }
            }

            return null;
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

        internal static void FlushCaches()
        {
            _pocoDatas.Flush();
        }
    }
}