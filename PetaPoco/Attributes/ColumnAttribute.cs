// <copyright file="ColumnAttribute.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;

namespace PetaPoco
{
    /// <summary>
    ///     Represents the attribute which decorates a poco property and marks the property as a column and optionally supplies
    ///     the DB column name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        ///     The column name.
        /// </summary>
        /// <returns>
        ///     The column name.
        /// </returns>
        public string Name { get; set; }

        /// <summary>
        ///     The column name.
        /// </summary>
        /// <returns>
        ///     The column name.
        /// </returns>
        public bool ForceToUtc { get; set; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ColumnAttribute" />.
        /// </summary>
        public ColumnAttribute()
        {
            ForceToUtc = false;
        }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ColumnAttribute" />.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnAttribute(string name)
        {
            Name = name;
            ForceToUtc = false;
        }
    }
}