using PetaPoco.Core;
using PetaPoco.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    ///     This static class manages registation of IMapper instances with PetaPoco
    /// </summary>
    public static class Mappers
    {
        private static ConcurrentDictionary<object, Lazy<IMapper>> _mappers = new ConcurrentDictionary<object, Lazy<IMapper>>();

        /// <summary>
        ///     Registers a mapper for all types in a specific assembly
        /// </summary>
        /// <param name="assembly">The assembly whose types are to be managed by this mapper</param>
        /// <param name="mapper">The IMapper implementation</param>
        public static bool Register(Assembly assembly, IMapper mapper) => RegisterInternal(assembly, mapper);

        /// <summary>
        ///     Registers a mapper for a single POCO type
        /// </summary>
        /// <param name="type">The type to be managed by this mapper</param>
        /// <param name="mapper">The IMapper implementation</param>
        public static bool Register(Type type, IMapper mapper) => RegisterInternal(type, mapper);

        /// <summary>
        ///     Remove all mappers for all types in a specific assembly
        /// </summary>
        /// <param name="assembly">The assembly whose mappers are to be revoked</param>
        public static bool Revoke(Assembly assembly) => RevokeInternal(assembly);

        /// <summary>
        ///     Remove the mapper for a specific type
        /// </summary>
        /// <param name="type">The type whose mapper is to be removed</param>
        public static bool Revoke(Type type) => RevokeInternal(type);

        /// <summary>
        ///     Revoke an instance of a mapper
        /// </summary>
        /// <param name="mapper">The IMapper to be revkoed</param>
        public static bool Revoke(IMapper mapper)
        {
            var ret = false;
            var m = _mappers.FirstOrDefault(v => v.Value.Value == mapper);
            var key = m.Equals(default(KeyValuePair<object, Lazy<IMapper>>)) ? null : m.Key;
            if (key != null)
            {
                ret = _mappers.TryRemove(key, out var _);
                if (ret) FlushCaches();
            }
            return ret;
        }

        /// <summary>
        ///     Revokes all registered mappers.
        /// </summary>
        public static void RevokeAll()
        {
            _mappers.Clear();
            FlushCaches();
        }

        /// <summary>
        ///     Retrieve the IMapper implementation to be used for a specified POCO type.
        /// </summary>
        /// <param name="entityType">The entity type to get the mapper for.</param>
        /// <param name="defaultMapper">The default mapper to use when none is registered for the type.</param>
        /// <returns>The mapper for the given type.</returns>
        public static IMapper GetMapper(Type entityType, IMapper defaultMapper) => _mappers.TryGetValue(entityType, out var m) ? m.Value : defaultMapper;

        private static bool RegisterInternal(object typeOrAssembly, IMapper mapper)
        {
            var ret = _mappers.TryAdd(typeOrAssembly, new Lazy<IMapper>(() => mapper));
            if (ret) FlushCaches();
            return ret;
        }

        private static bool RevokeInternal(object typeOrAssembly)
        {
            var ret = _mappers.TryRemove(typeOrAssembly, out var _);
            if (ret) FlushCaches();
            return ret;
        }

        private static void FlushCaches()
        {
            // Whenever a mapper is registered or revoked, we have to assume any generated code is no longer valid.
            // Since this should be a rare occurrence, the simplest approach is to simply dump everything and start over.
            MultiPocoFactory.FlushCaches();
            PocoData.FlushCaches();
        }
    }
}