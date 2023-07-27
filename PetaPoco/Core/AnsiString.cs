namespace PetaPoco
{
    /// <summary>
    /// Wraps a Unicode string that will be stored in a <c>VARCHAR</c> DB column as an <see cref="System.Data.DbType.AnsiString"/>.
    /// </summary>
    /// <remarks>
    /// Using this type for a column-mapped POCO property is equivalent to decorating a <see cref="string"/> property with <see cref="ColumnAttribute.ForceToAnsiString"/>.
    /// </remarks>
    public class AnsiString
    {
        /// <summary>
        /// Gets the Unicode string value wrapped by this instance.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnsiString"/> class with the specified string.
        /// </summary>
        /// <param name="value">The string to be stored in the database as an <see cref="System.Data.DbType.AnsiString"/>.</param>
        public AnsiString(string value) => Value = value;

        /// <summary>
        /// Explicitly converts a Unicode string to an <see cref="AnsiString"/> instance.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>An AnsiString instance containing the wrapped <see cref="string"/> value.</returns>
        public static explicit operator AnsiString(string value) => new AnsiString(value);
    }
}
