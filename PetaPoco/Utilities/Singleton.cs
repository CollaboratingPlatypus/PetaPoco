// <copyright file="Singleton.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

namespace PetaPoco.Internal
{
    internal static class Singleton<T> where T : new()
    {
        public static T Instance = new T();
    }
}