// <copyright file="StandardMapper.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    ///     StandardMapper is the default implementation of IMapper used by PetaPoco
    /// </summary>
    public class StandardMapper : IMapper
    {
        /// <summary>
        ///     Constructs a TableInfo for a POCO by reading its attribute data
        /// </summary>
        /// <param name="pocoType">The POCO Type</param>
        /// <returns></returns>
        public virtual TableInfo GetTableInfo(Type pocoType)
        {
            return TableInfo.FromPoco(pocoType);
        }

        /// <summary>
        ///     Constructs a ColumnInfo for a POCO property by reading its attribute data
        /// </summary>
        /// <param name="pocoProperty"></param>
        /// <returns></returns>
        public virtual ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
        {
            return ColumnInfo.FromProperty(pocoProperty);
        }

        public virtual Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType)
        {
            return null;
        }

        public virtual Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
        {
            return null;
        }
    }
}