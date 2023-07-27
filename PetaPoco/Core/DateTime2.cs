using System;

namespace PetaPoco
{
    /// <summary>
    /// Wraps a <see cref="DateTime"/> that will be stored in the database as a <see cref="System.Data.DbType.DateTime2"/>.
    /// </summary>
    /// <remarks>
    /// Using this type for a column-mapped POCO property is equivalent to decorating a DateTime property with <see cref="ColumnAttribute.ForceToDateTime2"/>.
    /// <para><see cref="System.Data.DbType.DateTime2">DbType.DateTime2</see> is a data type used by SQL DBs with a larger date range and fractional second precision than <see cref="System.Data.DbType.DateTime">DbType.DateTime</see>.</para>
    /// </remarks>
    public class DateTime2
    {
        /// <summary>
        /// Gets the <see cref="DateTime"/> value wrapped by this instance.
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Initializes a new instance of the DateTime2 class with the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The DateTime to be stored in the database as a <see cref="System.Data.DbType.DateTime2"/>.</param>
        public DateTime2(DateTime value) => Value = value;

        /// <summary>
        /// Explicitly converts a <see cref="DateTime"/> to a <see cref="DateTime2"/> instance.
        /// </summary>
        /// <param name="value">The DateTime value to convert.</param>
        /// <returns>A DateTime2 instance containing the wrapped <see cref="DateTime"/> value.</returns>
        public static explicit operator DateTime2(DateTime value) => new DateTime2(value);
    }
}
