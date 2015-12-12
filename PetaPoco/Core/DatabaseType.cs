// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/06</date>

using System;
using System.Data;
using System.Linq;
using PetaPoco.DatabaseTypes;

namespace PetaPoco.Internal
{
    public interface IDatabaseType
    {
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

    /// <summary>
    ///     Base class for DatabaseType handlers - provides default/common handling for different database engines
    /// </summary>
    internal abstract class DatabaseType : IDatabaseType
    {
        /// <summary>
        ///     Gets a flag for whether the DB has native support for GUID/UUID.
        /// </summary>
        public virtual bool HasNativeGuidSupport
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns the prefix used to delimit parameters in SQL query strings.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual string GetParameterPrefix(string connectionString)
        {
            return "@";
        }

        /// <summary>
        ///     Converts a supplied C# object value into a value suitable for passing to the database
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public virtual object MapParameterValue(object value)
        {
            if (value is bool)
            {
                return ((bool) value) ? 1 : 0;
            }

            return value;
        }

        /// <summary>
        ///     Called immediately before a command is executed, allowing for modification of the IDbCommand before it's passed to
        ///     the database provider
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void PreExecute(IDbCommand cmd)
        {
        }

        /// <summary>
        ///     Builds an SQL query suitable for performing page based queries to the database
        /// </summary>
        /// <param name="skip">The number of rows that should be skipped by the query</param>
        /// <param name="take">The number of rows that should be retruend by the query</param>
        /// <param name="parts">The original SQL query after being parsed into it's component parts</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL query</param>
        /// <returns>The final SQL query that should be executed.</returns>
        public virtual string BuildPageQuery(long skip, long take, PagingHelper.SQLParts parts, ref object[] args)
        {
            var sql = string.Format("{0}\nLIMIT @{1} OFFSET @{2}", parts.sql, args.Length, args.Length + 1);
            args = args.Concat(new object[] {take, skip}).ToArray();
            return sql;
        }

        /// <summary>
        ///     Returns an SQL Statement that can check for the existence of a row in the database.
        /// </summary>
        /// <returns></returns>
        public virtual string GetExistsSql()
        {
            return "SELECT COUNT(*) FROM {0} WHERE {1}";
        }

        /// <summary>
        ///     Escape a tablename into a suitable format for the associated database provider.
        /// </summary>
        /// <param name="tableName">
        ///     The name of the table (as specified by the client program, or as attributes on the associated
        ///     POCO class.
        /// </param>
        /// <returns>The escaped table name</returns>
        public virtual string EscapeTableName(string tableName)
        {
            // Assume table names with "dot" are already escaped
            return tableName.IndexOf('.') >= 0 ? tableName : EscapeSqlIdentifier(tableName);
        }

        /// <summary>
        ///     Escape and arbitary SQL identifier into a format suitable for the associated database provider
        /// </summary>
        /// <param name="str">The SQL identifier to be escaped</param>
        /// <returns>The escaped identifier</returns>
        public virtual string EscapeSqlIdentifier(string str)
        {
            return string.Format("[{0}]", str);
        }

        /// <summary>
        ///     Return an SQL expression that can be used to populate the primary key column of an auto-increment column.
        /// </summary>
        /// <param name="ti">Table info describing the table</param>
        /// <returns>An SQL expressions</returns>
        /// <remarks>See the Oracle database type for an example of how this method is used.</remarks>
        public virtual string GetAutoIncrementExpression(TableInfo ti)
        {
            return null;
        }

        /// <summary>
        ///     Returns an SQL expression that can be used to specify the return value of auto incremented columns.
        /// </summary>
        /// <param name="primaryKeyName">The primary key of the row being inserted.</param>
        /// <returns>An expression describing how to return the new primary key value</returns>
        /// <remarks>See the SQLServer database provider for an example of how this method is used.</remarks>
        public virtual string GetInsertOutputClause(string primaryKeyName)
        {
            return string.Empty;
        }

        /// <summary>
        ///     Performs an Insert operation
        /// </summary>
        /// <param name="db">The calling Database object</param>
        /// <param name="cmd">The insert command to be executed</param>
        /// <param name="primaryKeyName">The primary key of the table being inserted into</param>
        /// <returns>The ID of the newly inserted record</returns>
        public virtual object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return db.ExecuteScalarHelper(cmd);
        }

        /// <summary>
        ///     Look at the type and provider name being used and instantiate a suitable DatabaseType instance.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="providerName">The provider name.</param>
        /// <returns>The database type.</returns>
        public static DatabaseType Resolve(string typeName, string providerName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                // Try using type name first (more reliable)
                if (typeName.StartsWith("MySql"))
                    return Singleton<MySqlDatabaseType>.Instance;
                if (typeName.StartsWith("SqlCe"))
                    return Singleton<SqlServerCEDatabaseType>.Instance;
                if (typeName.StartsWith("Npgsql") || typeName.StartsWith("PgSql"))
                    return Singleton<PostgreSQLDatabaseType>.Instance;
                if (typeName.StartsWith("Oracle"))
                    return Singleton<OracleDatabaseType>.Instance;
                if (typeName.StartsWith("SQLite"))
                    return Singleton<SQLiteDatabaseType>.Instance;
                if (typeName.StartsWith("System.Data.SqlClient."))
                    return Singleton<SqlServerDatabaseType>.Instance;
            }

            if (!string.IsNullOrEmpty(providerName))
            {
                // Try again with provider name
                if (providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return Singleton<MySqlDatabaseType>.Instance;
                if (providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return Singleton<SqlServerCEDatabaseType>.Instance;
                if (providerName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return Singleton<PostgreSQLDatabaseType>.Instance;
                if (providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return Singleton<OracleDatabaseType>.Instance;
                if (providerName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return Singleton<SQLiteDatabaseType>.Instance;
            }

            // Assume SQL Server
            return Singleton<SqlServerDatabaseType>.Instance;
        }
    }
}