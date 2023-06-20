namespace PetaPoco
{
    /// <summary>
    /// Wraps a Unicode string that will be stored in a <c>VARCHAR</c> DB column as an <see cref="System.Data.DbType.AnsiString"/>.
    /// </summary>
    /// <remarks>
    /// Using this type for a column-mapped POCO property is equivalent to decorating a String property with <see cref="ColumnAttribute.ForceToAnsiString"/>.
    /// </remarks>
    public class AnsiString
    {
        /// <summary>
        /// Gets the Unicode <see cref="string"/> value wrapped by this intance.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnsiString"/> class with the given Unicode string.
        /// </summary>
        /// <param name="value">The Unicode <see cref="string"/> to be converted to ANSI before being passed to the DB as an <see cref="System.Data.DbType.AnsiString"/>.</param>
        public AnsiString(string value) => Value = value;

        /// <summary>
        /// Explicitly converts a Unicode string to an <see cref="AnsiString"/> instance.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>An <see cref="AnsiString" /> object that wraps the given <paramref name="value"/>.</returns>
        public static explicit operator AnsiString(string value) => new AnsiString(value);
    }
}
