// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/06/29</date>

using System;

namespace PetaPoco
{
    /// <summary>
    ///     Represents an attribute, which when applied to a Poco class, specifies the the DB table name which it maps to
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        ///     The table nane of the database that this entity maps to.
        /// </summary>
        /// <returns>
        ///     The table nane of the database that this entity maps to.
        /// </returns>
        public string Value { get; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="TableNameAttribute" />.
        /// </summary>
        /// <param name="tableName">The table nane of the database that this entity maps to.</param>
        public TableNameAttribute(string tableName) => Value = tableName;
    }
}