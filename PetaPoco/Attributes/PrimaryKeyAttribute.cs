﻿// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/30</date>

using System;

namespace PetaPoco
{
    /// <summary>
    ///     Is an attribute, which when applied to a Poco class, specifies primary key column. Additionally, specifies whether
    ///     the column is auto incrementing and the optional sequence name for Oracle sequence columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        ///     The column name.
        /// </summary>
        /// <returns>
        ///     The column name.
        /// </returns>
        public string[] Value { get; private set; }

        /// <summary>
        ///     The sequence name.
        /// </summary>
        /// <returns>
        ///     The sequence name.
        /// </returns>
        public string SequenceName { get; set; }

        /// <summary>
        ///     A flag which specifies if the primary key is auto incrementing.
        /// </summary>
        /// <returns>
        ///     True if the primary key is auto incrementing; else, False.
        /// </returns>
        public bool AutoIncrement { get; set; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="PrimaryKeyAttribute" />.
        /// </summary>
        /// <param name="primaryKey">The name of the primary key column.</param>
        public PrimaryKeyAttribute(params string[] primaryKey)
        {
            Value = primaryKey;
            AutoIncrement = true;
        }
    }
}