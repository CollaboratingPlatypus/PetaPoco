using System;

namespace PetaPoco
{
    /// <summary>
    /// The PrimaryKeyAttribute class defines an attribute for POCO properties that map to primary key columns in the database.
    /// </summary>
    /// <remarks>
    /// The PrimaryKeyAttribute, when used in a POCO class, designates the decorated property as the primary key column in the database. It can also be used to override the default mapped column name for the primary key, mark the column as auto-incrementing, and optionally assign a sequence name for Oracle sequence columns.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// Gets the column name in the database.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets or sets the optional sequence name, for Oracle databases.
        /// </summary>
        public string SequenceName { get; set; }

        /// <summary>
        /// Gets or sets whether the primary key column represented by this property in the database is auto-incrementing.
        /// Default is <see langword="true"/>.
        /// </summary>
        public bool AutoIncrement { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class with the specified column name.
        /// </summary>
        /// <param name="primaryKeyName">The database table's primary key column name that this property maps to.</param>
        public PrimaryKeyAttribute(string primaryKeyName) => Value = primaryKeyName;
    }
}
