using System;

namespace PetaPoco
{
    /// <summary>
    /// Provides extension methods for the <see cref="DateTime2"/> class.
    /// </summary>
    public static class DateTime2Extensions
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> to its <see cref="DateTime2"/> representation.
        /// </summary>
        /// <param name="value">The DateTime object to be converted.</param>
        /// <returns>A <see cref="DateTime2"/> object containing the converted <paramref name="value"/>.</returns>
        public static DateTime2 ToDateTime2(this DateTime value) => new DateTime2(value);

        /// <summary>
        /// Parses a string to its <see cref="DateTime2"/> representation.
        /// </summary>
        /// <param name="value">The string representing a date and time to be converted.</param>
        /// <returns>A <see cref="DateTime2"/> object containing the parsed <paramref name="value"/>.</returns>
        /// <inheritdoc cref="DateTime.Parse(string)"/>
        public static DateTime2 ToDateTime2(this string value) => new DateTime2(DateTime.Parse(value));
    }
}
