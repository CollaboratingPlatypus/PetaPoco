using System;

namespace PetaPoco
{
    /// <summary>
    ///     Represents an attribute which can decorate a POCO property to mark the property as a column. It may also optionally
    ///     supply the DB column name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        ///     The SQL name of the column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     True if time and date values returned through this column should be forced to UTC DateTimeKind. (no conversion is
        ///     applied - the Kind of the DateTime property
        ///     is simply set to DateTimeKind.Utc instead of DateTimeKind.Unknown.
        /// </summary>
        public bool ForceToUtc { get; set; }

        /// <summary>
        ///     The insert template. If not null, this template is used for generating the insert section instead of the default
        ///     string.Format("{0}{1}", paramPrefix, index"). Setting this allows DB related interactions, such as "CAST({0}{1} AS
        ///     json)"
        /// </summary>
        public string InsertTemplate { get; set; }

        /// <summary>
        ///     The update template. If not null, this template is used for generating the update section instead of the default
        ///     string.Format("{0} = {1}{2}", colName, paramPrefix, index"). Setting this allows DB related interactions, such as
        ///     "{0} = CAST({1}{2} AS
        ///     json)"
        /// </summary>
        public string UpdateTemplate { get; set; }
        /// <summary>
        ///     True if AnsiString / VARCHAR column
        /// </summary>
        public bool ForceToAnsiString { get; set; }
        /// <summary>
        ///     True if DateTime2 column
        /// </summary>
        public bool ForceToDateTime2 { get; set; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ColumnAttribute" />.
        /// </summary>
        public ColumnAttribute()
        {
        }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ColumnAttribute" />.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}