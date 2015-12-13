// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

namespace PetaPoco.Utilities
{
    /// <summary>
    ///     Presents the SQL parts.
    /// </summary>
    public struct SQLParts
    {
        /// <summary>
        ///     The SQL.
        /// </summary>
        public string Sql;

        /// <summary>
        ///     The SQL count.
        /// </summary>
        public string SqlCount;

        /// <summary>
        ///     The SQL Select
        /// </summary>
        public string SqlSelectRemoved;

        /// <summary>
        ///     The SQL Order By
        /// </summary>
        public string SqlOrderBy;
    }
}