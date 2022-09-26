using System;
using System.Collections.Generic;

namespace PetaPoco.Internal
{
    internal static class EnumMapper
    {
        private static Cache<Type, Dictionary<string, object>> _types = new Cache<Type, Dictionary<string, object>>();

        public static object EnumFromString(Type enumType, string value)
        {
            Dictionary<string, object> map = _types.GetOrAdd(enumType, () =>
            {
                var values = Enum.GetValues(enumType);

                var newmap = new Dictionary<string, object>(values.Length, StringComparer.InvariantCultureIgnoreCase);

                foreach (var v in values)
                {
                    newmap.Add(v.ToString(), v);
                }

                return newmap;
            });

            try
            {
                return map[value];
            }
            catch (KeyNotFoundException inner)
            {
                throw new KeyNotFoundException(
                    $"Requested value '{value}' was not found in enum {enumType.Name}.",
                    inner);
            }
        }
    }
}