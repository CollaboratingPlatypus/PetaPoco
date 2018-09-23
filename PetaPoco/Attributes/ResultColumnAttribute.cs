// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/06/29</date>

using System;

namespace PetaPoco
{
    public enum IncludeInAutoSelect { No, Yes }

    /// <summary>
    ///     Represents an attribute which can decorate a poco property as a result only column. A result only column is a
    ///     column that is only populated in queries and is not used for updates or inserts operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        public IncludeInAutoSelect IncludeInAutoSelect { get; set; }

        public ResultColumnAttribute()
        {
        }

        public ResultColumnAttribute(string name) 
            : this(name, IncludeInAutoSelect.No)
        {
        }

        public ResultColumnAttribute(IncludeInAutoSelect includeInAutoSelect)
            : this(null, includeInAutoSelect)
        {
        }

        public ResultColumnAttribute(string name, IncludeInAutoSelect includeInAutoSelect)
            : base(name)
        {
            IncludeInAutoSelect = includeInAutoSelect;
        }
    }
}