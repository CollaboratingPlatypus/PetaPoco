using System;

namespace PetaPoco.Tests.Integration
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type source)
            => source.IsGenericType && source.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}