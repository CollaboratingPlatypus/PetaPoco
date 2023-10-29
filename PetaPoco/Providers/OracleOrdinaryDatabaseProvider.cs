using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetaPoco.Providers
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc/><br />
    /// It utilizes the builtin <see cref="Core.ExpandoPoco"/> for dynamic objects to accomodate the use of ordinary identifiers.<br />
    /// For delimited identifiers, use <see cref="OracleDatabaseProvider"/>
    /// </remarks>
    public class OracleOrdinaryDatabaseProvider : OracleDatabaseProvider
    {
        /// <inheritdoc/>
        public override bool UseOrdinaryIdentifiers => true;
    }
}
