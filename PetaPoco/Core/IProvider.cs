// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using PetaPoco.Utilities;

namespace PetaPoco.Core
{
    /// <summary>
    ///     Represents a contract for a database type provider.
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        ///     Gets the <seealso cref="IPagingHelper" /> this provider supplies.
        /// </summary>
        IPagingHelper PagingUtility { get; }

        /// <summary>
        ///     Escape a tablename into a suitable format for the associated database provider.
        /// </summary>
        /// <param name="tableName">
        ///     The name of the table (as specified by the client program, or as attributes on the associated
        ///     POCO class.
        /// </param>
        /// <returns>The escaped table name</returns>
        string EscapeTableName(string tableName);

        /// <summary>
        ///     Escape and arbitary SQL identifier into a format suitable for the associated database provider
        /// </summary>
        /// <param name="str">The SQL identifier to be escaped</param>
        /// <returns>The escaped identifier</returns>
        string EscapeSqlIdentifier(string str);
    }
}