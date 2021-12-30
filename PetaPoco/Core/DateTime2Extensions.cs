using System;

namespace PetaPoco
{
    public static class DateTime2Extensions
    {
        /// <summary>
        /// Converts an DateTime to its <see cref="DateTime2"/> representation
        /// </summary>
        /// <param name="dt">The DateTime to be converted</param>
        /// <returns></returns>
        public static DateTime2 ToDateTime2(this DateTime dt) => new DateTime2(dt);
        /// <summary>
        /// Parse a string to its <see cref="DateTime2"/> representation
        /// </summary>
        /// <param name="inputStr">The string to be converted</param>
        /// <returns></returns>
        public static DateTime2 ToDateTime2(this string inputStr) => new DateTime2(DateTime.Parse(inputStr));
    }
}
