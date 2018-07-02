// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;

namespace PetaPoco.Tests.Integration
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type source)
        {
            return (source.IsGenericType && source.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}