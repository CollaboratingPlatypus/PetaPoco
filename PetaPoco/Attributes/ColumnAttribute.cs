using System;

namespace PetaPoco
{
    /// <summary>
    /// The ColumnAttribute class defines an attribute for POCO properties that map to a database column.
    /// </summary>
    /// <remarks>
    /// This attribute can be used to override the default mapped column name, provide hints to PetaPoco for how to treat DateTime and String columns, and further customize INSERT and UPDATE operations with the use of string templates.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the database column name this property maps to.
        /// </summary>
        /// <value>When not <see langword="null"/>, overrides this property's inflected column name from the mapper.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether the column is of type <see cref="AnsiString" /> (SQL DB data type <c>VARCHAR</c>).
        /// </summary>
        /// <remarks>
        /// For use with <see cref="string"/> properties. This property is implicitly <see langword="true"/> for properties of type <see cref="AnsiString"/>.
        /// </remarks>
        public bool ForceToAnsiString { get; set; }

        /// <summary>
        /// Gets or sets whether the column is of type <see cref="DateTime2" />.
        /// </summary>
        /// <remarks>
        /// For use with <see cref="DateTime"/> properties. This property is implicitly <see langword="true"/> for properties of type <see cref="DateTime2"/>.
        /// </remarks>
        public bool ForceToDateTime2 { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="System.Data.DbType.DateTime">DbType.DateTime</see> or <see cref="System.Data.DbType.DateTime2">DbType.DateTime2</see> values in this DB column are always UTC.
        /// </summary>
        /// <remarks>
        /// No conversion is applied - the <see cref="DateTimeKind" /> of the POCO property's underlying <see cref="DateTime"/> value is simply set to correctly reflect the UTC timezone as an invariant.
        /// </remarks>
        /// <value>If <see langword="true"/>, the underlying <see cref="DateTime"/>'s <see cref="DateTimeKind"/> property is set to <see cref="DateTimeKind.Utc">Utc</see>; otherwise, the default is used (<see cref="DateTimeKind.Unspecified">Unspecified</see>).</value>
        public bool ForceToUtc { get; set; }

        /// <summary>
        /// Gets or sets the INSERT string template.
        /// </summary>
        /// <remarks>
        /// When set, this template is used for generating the INSERT portion of the SQL statement instead of the default
        /// <br/><c>String.Format("{0}{1}", paramPrefix, index)</c>.
        /// <para>Setting this allows database-related interactions, such as:
        /// <br/><c>String.Format("CAST({0}{1} AS JSON)", paramPrefix, index)</c>.</para>
        /// </remarks>
        public string InsertTemplate { get; set; }

        /// <summary>
        /// Gets or sets the UPDATE string template.
        /// </summary>
        /// <remarks>
        /// When set, this template is used for generating the UPDATE portion of the SQL statement instead of the default
        /// <br/><c>String.Format("{0} = {1}{2}", colName, paramPrefix, index)</c>.
        /// <para>Setting this allows database-related interactions, such as:
        /// <br/><c>String.Format("{0} = CAST({1}{2} AS JSON)", colName, paramPrefix, index)</c></para>
        /// </remarks>
        public string UpdateTemplate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class with default values.
        /// </summary>
        public ColumnAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class with the specified column name.
        /// </summary>
        /// <param name="column">The name of the database column associated with this property.</param>
        public ColumnAttribute(string column)
        {
            Name = column;
        }
    }
}
