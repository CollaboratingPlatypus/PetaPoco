using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetaPoco
{
    public static class AnsiStringExtensions
    {
        /// <summary>
        /// Converts an object to its <see cref="AnsiString"/> representation
        /// </summary>
        /// <param name="o">The object to be converted</param>
        /// <returns></returns>
        public static AnsiString ToAnsiString(this object o) => new AnsiString(o.ToString());
    }
}
