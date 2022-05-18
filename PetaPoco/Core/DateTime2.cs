using System;

namespace PetaPoco
{
    /// <summary>
    ///     Wrap DateTime in an instance of this class to force use of DBType.DateTime2
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
        /// <param name="dt">The C# DateTime to be converted to DateTime2 before being passed to the DB</param>
        public DateTime2(DateTime dt)
            => Value = dt;

        public static explicit operator DateTime2(DateTime dt) => new DateTime2(dt);
    }
}