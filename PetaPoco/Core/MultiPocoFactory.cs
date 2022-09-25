using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using PetaPoco.Core;

namespace PetaPoco.Internal
{
    internal class MultiPocoFactory
    {
        // Various cached stuff
        private static readonly Cache<Tuple<Type, ArrayKey<Type>, string, string, int>, object> MultiPocoFactories = new Cache<Tuple<Type, ArrayKey<Type>, string, string, int>, object>();

        private static readonly Cache<ArrayKey<Type>, object> AutoMappers = new Cache<ArrayKey<Type>, object>();

        // Instance data used by the Multipoco factory delegate - essentially a list of the nested poco factories to call
        private List<Delegate> _delegates;

        public Delegate GetItem(int index)
        {
            return _delegates[index];
        }

        // Automagically guess the property relationships between various POCOs and create a delegate that will set them up
        public static object GetAutoMapper(Type[] types)
        {
            // Build a key
            var key = new ArrayKey<Type>(types);

            return AutoMappers.GetOrAdd(key, () =>
            {
                // Create a method
                var m = new DynamicMethod("petapoco_automapper", types[0], types, true);
                var il = m.GetILGenerator();

                for (var i = 1; i < types.Length; i++)
                {
                    var handled = false;
                    for (var j = i - 1; j >= 0; j--)
                    {
                        // Find the property
                        var candidates = (from p in types[j].GetProperties() where p.PropertyType == types[i] select p).ToArray();
                        if (!candidates.Any())
                            continue;
                        if (candidates.Length > 1)
                            throw new InvalidOperationException(string.Format("Can't auto join {0} as {1} has more than one property of type {0}", types[i], types[j]));

                        // Generate code
                        il.Emit(OpCodes.Ldarg_S, j);
                        il.Emit(OpCodes.Ldarg_S, i);
                        il.Emit(OpCodes.Callvirt, candidates.First().GetSetMethod(true));
                        handled = true;
                    }

                    if (!handled)
                        throw new InvalidOperationException(string.Format("Can't auto join {0}", types[i]));
                }

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);

                // Cache it
                return m.CreateDelegate(Expression.GetFuncType(types.Concat(types.Take(1)).ToArray()));
            });
        }

        // Find the split point in a result set for two different POCOs and return the POCO factory for the first
        private static Delegate FindSplitPoint(Type typeThis, Type typeNext, string connectionString, string sql, IDataReader r, ref int pos, IMapper defaultMapper)
        {
            // Last?
            if (typeNext == null)
                return PocoData.ForType(typeThis, defaultMapper).GetFactory(sql, connectionString, pos, r.FieldCount - pos, r, defaultMapper);

            // Get PocoData for the two types
            var pdThis = PocoData.ForType(typeThis, defaultMapper);
            var pdNext = PocoData.ForType(typeNext, defaultMapper);

            // Find split point
            var firstColumn = pos;
            var usedColumns = new Dictionary<string, bool>();
            for (; pos < r.FieldCount; pos++)
            {
                // Split if field name has already been used, or if the field doesn't exist in current poco but does in the next
                var fieldName = r.GetName(pos);
                if (usedColumns.ContainsKey(fieldName) || (!pdThis.Columns.ContainsKey(fieldName) && pdNext.Columns.ContainsKey(fieldName)))
                {
                    return pdThis.GetFactory(sql, connectionString, firstColumn, pos - firstColumn, r, defaultMapper);
                }

                usedColumns.Add(fieldName, true);
            }

            throw new InvalidOperationException(string.Format("Couldn't find split point between {0} and {1}", typeThis, typeNext));
        }

        // Create a multi-poco factory
        private static Func<IDataReader, object, TRet> CreateMultiPocoFactory<TRet>(Type[] types, string connectionString, string sql, IDataReader r, IMapper defaultMapper)
        {
            var m = new DynamicMethod("petapoco_multipoco_factory", typeof(TRet), new[] { typeof(MultiPocoFactory), typeof(IDataReader), typeof(object) },
                typeof(MultiPocoFactory));
            var il = m.GetILGenerator();

            // Load the callback
            il.Emit(OpCodes.Ldarg_2);

            // Call each delegate
            var dels = new List<Delegate>();
            var pos = 0;
            for (var i = 0; i < types.Length; i++)
            {
                // Add to list of delegates to call
                var del = FindSplitPoint(types[i], i + 1 < types.Length ? types[i + 1] : null, connectionString, sql, r, ref pos, defaultMapper);
                dels.Add(del);

                // Get the delegate
                il.Emit(OpCodes.Ldarg_0); // callback,this
                il.Emit(OpCodes.Ldc_I4, i); // callback,this,Index
                il.Emit(OpCodes.Callvirt, typeof(MultiPocoFactory).GetMethod("GetItem")); // callback,Delegate
                il.Emit(OpCodes.Ldarg_1); // callback,delegate, datareader

                // Call Invoke
                var tDelInvoke = del.GetType().GetMethod("Invoke");
                il.Emit(OpCodes.Callvirt, tDelInvoke); // Poco left on stack
            }

            // By now we should have the callback and the N pocos all on the stack.  Call the callback and we're done
            il.Emit(OpCodes.Callvirt, Expression.GetFuncType(types.Concat(new[] { typeof(TRet) }).ToArray()).GetMethod("Invoke"));
            il.Emit(OpCodes.Ret);

            // Finish up
            return (Func<IDataReader, object, TRet>) m.CreateDelegate(typeof(Func<IDataReader, object, TRet>), new MultiPocoFactory() { _delegates = dels });
        }

        internal static void FlushCaches()
        {
            MultiPocoFactories.Flush();
            AutoMappers.Flush();
        }

        // Get (or create) the multi-poco factory for a query
        public static Func<IDataReader, object, TRet> GetFactory<TRet>(Type[] types, string connectionString, string sql, IDataReader r, IMapper defaultMapper)
        {
            var key = Tuple.Create(typeof(TRet), new ArrayKey<Type>(types), connectionString, sql, r.FieldCount);

            return (Func<IDataReader, object, TRet>) MultiPocoFactories.GetOrAdd(key, () => CreateMultiPocoFactory<TRet>(types, connectionString, sql, r, defaultMapper));
        }
    }
}