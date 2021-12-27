using System;

namespace PetaPoco
{
    /// <summary>
    ///     Wrap strings in an instance of this class to force use of DBType.DateTime2
    /// </summary>
    public class DateTime2
    {
        /// <summary>
        ///     The DateTime value
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        ///     Constructs an DateTime2
        /// </summary>
        /// <param name="str">The C# string to be converted to ANSI before being passed to the DB</param>
        public DateTime2(DateTime str)
            => Value = str;
    }
}