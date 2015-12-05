// <copyright file="ResultColumnAttribute.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;

namespace PetaPoco
{
    /// <summary>
    ///     Represents the attribute which decorates a poco property as a result only column. A result only column is a column
    ///     that is only populated in queries and is not used for updates or inserts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultColumnAttribute" />.
        /// </summary>
        public ResultColumnAttribute()
        {
        }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultColumnAttribute" />.
        /// </summary>
        /// <param name="name">The name of the DB column.</param>
        public ResultColumnAttribute(string name)
            : base(name)
        {
        }
    }
}