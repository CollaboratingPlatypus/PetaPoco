using System;
using System.Collections.Generic;

namespace PetaPoco.Internal
{
    internal static class EnumMapper
    {
        private static Cache<Type, Dictionary<string, object>> _types = new Cache<Type, Dictionary<string, object>>();

        public static object EnumFromString(Type enumType, string value)
        {
            Dictionary<string, object> map = _types.Get(enumType, () =>
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
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException(
                    string.Format("Can not convert value `{0}` into {1}-Enum", value, enumType.Name));
            }
        }
    }
}