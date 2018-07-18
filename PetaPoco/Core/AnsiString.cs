// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/06/29</date>

namespace PetaPoco
{
    /// <summary>
    ///     Wrap strings in an instance of this class to force use of DBType.AnsiString
    /// </summary>
    public class AnsiString
    {
        /// <summary>
        ///     The string value
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Constructs an AnsiString
        /// </summary>
        /// <param name="str">The C# string to be converted to ANSI before being passed to the DB</param>
        public AnsiString(string str) => Value = str;
    }
}