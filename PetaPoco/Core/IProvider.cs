using System.Data;
using System.Data.Common;
#if ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif
using PetaPoco.Utilities;

namespace PetaPoco.Core
{
    /// <summary>
    /// Defines the contract for database providers that expose functionality for connecting to various types of databases.
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// Gets the <see cref="IPagingHelper"/> supplied by this provider.
        /// </summary>
        IPagingHelper PagingUtility { get; }

        /// <summary>
        /// Gets a flag indicating whether the DB has native support for GUID/UUID.
        /// </summary>
        bool HasNativeGuidSupport { get; }

        /// <summary>
        /// Escapes a table name into a suitable format for the associated database provider.
        /// </summary>
        /// <param name="tableName">The name of the table as specified by the client program, or as attributes on the associated POCO
        /// class.</param>
        /// <returns>The escaped table name.</returns>
        string EscapeTableName(string tableName);

        /// <summary>
        /// Escapes an arbitrary SQL identifier into a format suitable for the associated database provider.
        /// </summary>
        /// <param name="sqlIdentifier">The SQL identifier to be escaped.</param>
        /// <returns>The escaped identifier.</returns>
        string EscapeSqlIdentifier(string sqlIdentifier);

        /// <summary>
        /// Builds an SQL query suitable for performing page-based queries to the database.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="parts">The parsed SQL query parts.</param>
        /// <param name="args">The arguments to any embedded parameters in the SQL query.</param>
        /// <returns>The final SQL query to be executed.</returns>
        string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args);

        /// <summary>
        /// Converts the specified C# object value into a data type suitable for passing to the database.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        object MapParameterValue(object value);

        /// <summary>
        /// Called immediately before an SQL command is executed, allowing for modification of the command before being passed to the
        /// database provider.
        /// </summary>
        /// <param name="cmd">The SQL command to be executed.</param>
        void PreExecute(IDbCommand cmd);

        /// <summary>
        /// Returns an SQL statement that can check for the existence of a row in the database.
        /// </summary>
        /// <returns>The SQL statement.</returns>
        string GetExistsSql();

        /// <summary>
        /// Performs an Insert operation.
        /// </summary>
        /// <param name="db">The database instance that will execute the SQL command.</param>
        /// <param name="cmd">The SQL command to be executed.</param>
        /// <param name="primaryKeyName">The primary key column name for the table being inserted into.</param>
        /// <returns>The ID of the newly inserted record.</returns>
        object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName);

#if ASYNC
        /// <summary>
        /// Performs an Insert operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="db">The database instance that will execute the SQL command.</param>
        /// <param name="cmd">The SQL command to be executed.</param>
        /// <param name="primaryKeyName">The primary key column name for the table being inserted into.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the primary key of the new record.
        /// </returns>
        Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName);
#endif

        /// <summary>
        /// Returns an SQL expression that can be used to specify the return value of auto-incremented columns.
        /// </summary>
        /// <remarks>
        /// See the SQLServer database provider for an example of how this method is used.
        /// </remarks>
        /// <param name="primaryKeyName">The primary key column name of the row being inserted.</param>
        /// <returns>An expression describing how to return the new primary key value.</returns>
        string GetInsertOutputClause(string primaryKeyName);

        /// <summary>
        /// Returns the prefix used to delimit parameters in SQL query strings.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The provider's character for prefixing a query parameter.</returns>
        string GetParameterPrefix(string connectionString);

        /// <summary>
        /// Returns an SQL expression that can be used to populate the primary key column of an auto-increment column.
        /// </summary>
        /// <remarks>
        /// See the Oracle database type for an example of how this method is used.
        /// </remarks>
        /// <param name="tableInfo">The TableInfo instance describing the table.</param>
        /// <returns>An SQL expression.</returns>
        string GetAutoIncrementExpression(TableInfo tableInfo);

        /// <summary>
        /// Returns the DbProviderFactory.
        /// </summary>
        /// <returns>The DbProviderFactory factory.</returns>
        DbProviderFactory GetFactory();
    }
}
