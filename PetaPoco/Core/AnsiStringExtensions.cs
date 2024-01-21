namespace PetaPoco
{
    /// <summary>
    /// Provides extension methods for the <see cref="AnsiString"/> class.
    /// </summary>
    public static class AnsiStringExtensions
    {
        /// <summary>
        /// Converts an object to its <see cref="AnsiString"/> representation.
        /// </summary>
        /// <param name="value">The object to be converted.</param>
        /// <returns>An AnsiString object that wraps the given <paramref name="value"/>.</returns>
        public static AnsiString ToAnsiString(this object value) => new AnsiString(value.ToString());
    }
}
