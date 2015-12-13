// <copyright file="ExpandoColumn.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System.Collections.Generic;
using PetaPoco.Core;

namespace PetaPoco.Internal
{
    internal class ExpandoColumn : PocoColumn
    {
        public override void SetValue(object target, object val)
        {
            (target as IDictionary<string, object>)[ColumnName] = val;
        }

        public override object GetValue(object target)
        {
            object val = null;
            (target as IDictionary<string, object>).TryGetValue(ColumnName, out val);
            return val;
        }

        public override object ChangeType(object val)
        {
            return val;
        }
    }
}