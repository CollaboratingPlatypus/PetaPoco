// <copyright file="PocoColumn.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Reflection;

namespace PetaPoco.Core
{
    public class PocoColumn
    {
        public string ColumnName;
        public bool ForceToUtc;
        public PropertyInfo PropertyInfo;
        public bool ResultColumn;

        public virtual void SetValue(object target, object val)
        {
            PropertyInfo.SetValue(target, val, null);
        }

        public virtual object GetValue(object target)
        {
            return PropertyInfo.GetValue(target, null);
        }

        public virtual object ChangeType(object val)
        {
            return Convert.ChangeType(val, PropertyInfo.PropertyType);
        }
    }
}