using System;

namespace PetaPoco
{
    /// <summary>
    /// The TableNameAttribute class defines an attribute for POCO classes to specify a custom database table name that class should map to.
    /// </summary>
    /// <remarks>
    /// When decorating a class, the provided table name overrides the default inflected name of the active mapper.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// The database table name this POCO maps to.
        /// </summary>
        /// <value>Overrides this class's inflected table name from the mapper.</value>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableNameAttribute"/> class with the specified table name.
        /// <para/><inheritdoc cref="TableNameAttribute"/>
        /// </summary>
        /// <param name="tableName">The database table name this class maps to.</param>
        public TableNameAttribute(string tableName) => Value = tableName;
    }
}
